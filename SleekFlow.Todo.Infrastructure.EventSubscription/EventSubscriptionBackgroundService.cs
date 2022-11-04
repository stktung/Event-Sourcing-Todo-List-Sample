using EventStore.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SleekFlow.Todo.Domain;
using SleekFlow.Todo.Infrastructure.EmbeddedEventStoreDB;
using EventStorePersistentSubscriptionBase = EventStore.ClientAPI.EventStorePersistentSubscriptionBase;
using UserCredentials = EventStore.ClientAPI.SystemData.UserCredentials;

namespace SleekFlow.Todo.Infrastructure.EventSubscription;

public class EventSubscriptionBackgroundService : BackgroundService
{
    private readonly ILogger _logger;
    private readonly IEventStore _eventStore;
    private readonly EventStorePersistentSubscriptionsClient _subscriptionsClient;
    private readonly EventStore.ClientAPI.PersistentSubscriptionSettings _subscriptionSettings;
    private readonly string _groupName;
    private readonly string _streamName;
    private readonly string _logPrefix;
    private readonly TimeSpan _retryReconnectDelay;
    private bool _connected;
    private readonly TimeSpan _retryDelay;
    private readonly int _maxRetryCount;
    private readonly UserCredentials _userCredentials;
    
    protected Func<IEvent, HandlerResult> _subscriptionHandler;

    public EventSubscriptionBackgroundService(
        ILogger<EventSubscriptionBackgroundService> logger,
        IOptions<EventSubscriptionHandlerOptions> options, 
        IEventStore eventStore)
    {
        _logger = logger;
        _eventStore = eventStore;

        var subscriptionOptions = options.Value ?? throw new ArgumentNullException(nameof(options));

        _subscriptionsClient = subscriptionOptions.Client;
        _subscriptionSettings = subscriptionOptions.Settings;
        _groupName = subscriptionOptions.GroupName;
        _streamName = subscriptionOptions.StreamName;
        _logPrefix = $"[Stream: {_streamName}, Group: {_groupName}]";
        _retryReconnectDelay = subscriptionOptions.RetryReconnectDelay;
        _userCredentials = new UserCredentials(subscriptionOptions.UserName, subscriptionOptions.Password);
        _retryDelay = subscriptionOptions.RetryDelay;
        _maxRetryCount = subscriptionOptions.Settings.MaxRetryCount;
    }

    public override async Task StartAsync(CancellationToken token)
    {
        _logger.LogInformation($"Starting event subscription on '{_streamName}' ...");
        await base.StartAsync(token);
    }

    public override async Task StopAsync(CancellationToken token)
    {
        _logger.LogError($"Stopping event subscription on '{_streamName}' ...");
        _subscriptionsClient.Dispose();
        await base.StopAsync(token);
    }

    public override void Dispose()
    {
        _logger.LogError($"Disposing event subscription on '{_streamName}' ...");
        _subscriptionsClient.Dispose();
        base.Dispose();
    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        try
        {
            await UpdatePersistentSubscriptionAsync(token);
            await ConnectToPersistentSubscriptionAsync(token);
        }
        catch (Exception)
        {
            await CreatePersistentSubscriptionAsync(token);
            await ConnectToPersistentSubscriptionAsync(token);
        }

        while (!token.IsCancellationRequested)
            await Task.Delay(TimeSpan.FromHours(1), token);
    }

    private async Task EventAppearedAsync(EventStorePersistentSubscriptionBase subscription, EventStore.ClientAPI.ResolvedEvent resolvedEvent, int? retryCount)
    {
        try
        {
            if (retryCount > 0)
                _logger.LogInformation($"Retrying {resolvedEvent.Event.EventType} '{resolvedEvent.Event.EventId}' on subscription {_logPrefix} for {retryCount} time(s).");

            if (retryCount > _maxRetryCount)
            {
                var msg = $"Retry count '{retryCount}' exceeds max '{_maxRetryCount}' for subscription {_logPrefix}.";
                _logger.LogError(msg);
                subscription.Fail(resolvedEvent, EventStore.ClientAPI.PersistentSubscriptionNakEventAction.Park, msg);
                return;
            }

            var @event = GetEvent(resolvedEvent);

            var result = _subscriptionHandler(@event);
            if (!result.IsSuccess)
            {
                switch (result.Error!.Code)
                {
                    case nameof(Errors.SkipEvent):
                        _logger.LogInformation($"{result.Error!.Message} {{@ResolvedEvent}} {{@Event}}.", resolvedEvent, @event);
                        subscription.Fail(resolvedEvent, EventStore.ClientAPI.PersistentSubscriptionNakEventAction.Skip, result.Error!.Message);
                        break;
                    case nameof(Errors.RetryEvent):
                        _logger.LogInformation($"{result.Error!.Message} {{@ResolvedEvent}} {{@Event}}.", resolvedEvent, @event);
                        Thread.Sleep(_retryDelay);
                        subscription.Fail(resolvedEvent, EventStore.ClientAPI.PersistentSubscriptionNakEventAction.Retry, result.Error!.Message);
                        break;
                    default:
                        _logger.LogError($"Handle {{@ResolvedEvent}} by XXX with {{@Result}}.", resolvedEvent, result);
                        subscription.Fail(resolvedEvent, EventStore.ClientAPI.PersistentSubscriptionNakEventAction.Park, result.Error!.Message);
                        break;
                }
            }
            else
            {
                _logger.LogInformation($"Handled '{resolvedEvent.Event.EventNumber}@{resolvedEvent.Event.EventStreamId}' by XXX.");
                subscription.Acknowledge(resolvedEvent);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception in XXX. Retrying {{@ResolvedEvent}} in {_retryDelay.Seconds} second(s).", resolvedEvent);
            Thread.Sleep(_retryDelay);
            subscription.Fail(resolvedEvent, EventStore.ClientAPI.PersistentSubscriptionNakEventAction.Retry, ex.Message);
        }
    }
    
    private async Task SubscriptionDroppedAsync(
        EventStorePersistentSubscriptionBase subscription,
        EventStore.ClientAPI.SubscriptionDropReason reason,
        Exception? ex,
        CancellationToken token)
    {
        if (ex is null)
            _logger.LogWarning($"Subscription dropped for {_logPrefix} due to '{reason}'. Reconnecting ...");
        else
            _logger.LogWarning($"Subscription dropped for {_logPrefix} due to '{reason}'. {{@Exception}}. Reconnecting ...", ex);

        _connected = false;

        do
        {
            _logger.LogWarning($"Reconnecting event subscription on '{_streamName}' ...' {_logPrefix}.");
            try
            {
                await UpdatePersistentSubscriptionAsync(token);
                await ConnectToPersistentSubscriptionAsync(token);
            }
            catch (Exception exception)
            {
                _logger.LogError($"Subscription reconnection failed for {_logPrefix}. {{@Exception}}", exception);
            }

            if (!_connected) await Task.Delay(_retryReconnectDelay, token);
        } while (!_connected);
    }

    private async Task CreatePersistentSubscriptionAsync(CancellationToken token)
        => await _eventStore.Connection.CreatePersistentSubscriptionAsync(_streamName, _groupName,
            _subscriptionSettings, _userCredentials);

    private async Task UpdatePersistentSubscriptionAsync(CancellationToken token)
        => await _eventStore.Connection.UpdatePersistentSubscriptionAsync(
            _streamName,
            _groupName,
            _subscriptionSettings,
            _userCredentials);

    private async Task ConnectToPersistentSubscriptionAsync(CancellationToken token)
    {
        await _eventStore.Connection.ConnectToPersistentSubscriptionAsync(
            _streamName,
            _groupName,
            EventAppearedAsync,
            async (subscription, reason, exception)
                => await SubscriptionDroppedAsync(subscription, reason, exception, token),
            _userCredentials, 
            autoAck: false);

        _connected = true;

        _logger.LogInformation($"Subscription connected for {_logPrefix}.");
    }

    public IEvent GetEvent(EventStore.ClientAPI.ResolvedEvent resolvedEvent)
    {
        return new TodoCreatedEvent();
    }
    
}