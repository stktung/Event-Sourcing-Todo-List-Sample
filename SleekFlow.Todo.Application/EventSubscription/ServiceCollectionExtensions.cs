using EventStore.Client;

namespace SleekFlow.Todo.Application.EventSubscription
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTecEventSubscription<TBackgroundService>(
            this IServiceCollection services,
            Action<EventSubscriptionOptions> configure) where TBackgroundService : BackgroundService
        {
            if (configure is null)
                throw new ArgumentNullException(nameof(configure));

            var options = new EventSubscriptionOptions();
            configure(options);

            if (options.EventStoreConfigs is null)
                throw new InvalidOperationException(
                    $"'{nameof(options.EventStoreConfigs)}' is not set in '{nameof(EventSubscriptionOptions)}'.");
            var configs = options.EventStoreConfigs;
            if (string.IsNullOrEmpty(configs.Username))
                throw new InvalidOperationException(
                    $"'{nameof(configs.Username)}' is not configured in '{nameof(EventSubscriptionOptions)}'.");
            if (string.IsNullOrEmpty(configs.Password))
                throw new InvalidOperationException(
                    $"'{nameof(configs.Password)}' is not configured in '{nameof(EventSubscriptionOptions)}'.");
            if (string.IsNullOrEmpty(configs.ApplicationName))
                throw new InvalidOperationException(
                    $"'{nameof(configs.ApplicationName)}' is not configured in '{nameof(EventSubscriptionOptions)}'.");
            if (options.SubscribeToStream is null)
                throw new InvalidOperationException(
                    $"'{nameof(options.SubscribeToStream)}' is not configured in '{nameof(EventSubscriptionOptions)}'.");
            if (options.MaximumCheckPointCountOf < 1)
                throw new InvalidOperationException(
                    $"'{nameof(options.MaximumCheckPointCountOf)}' is not valid in '{nameof(EventSubscriptionOptions)}'.");
            if (options.MinimumCheckPointCountOf < 1)
                throw new InvalidOperationException(
                    $"'{nameof(options.MinimumCheckPointCountOf)}' is not valid in '{nameof(EventSubscriptionOptions)}'.");
            if (options.CheckPointAfterSeconds < 1)
                throw new InvalidOperationException(
                    $"'{nameof(options.CheckPointAfterSeconds)}' is not valid in '{nameof(EventSubscriptionOptions)}'.");
            if (options.SubscriptionReconnectRetryDelayInSeconds.HasValue &&
                options.SubscriptionReconnectRetryDelayInSeconds < 1)
                throw new InvalidOperationException(
                    $"'{nameof(options.SubscriptionReconnectRetryDelayInSeconds)}' is not valid in '{nameof(EventSubscriptionOptions)}'.");
            if (options.MaxRetryCount <= 0)
                throw new InvalidOperationException(
                    $"'{nameof(options.MaxRetryCount)}' must be greater than 0 in '{nameof(EventSubscriptionOptions)}'.");
            if (options.RetryDelay >= options.MessageTimeout)
                throw new InvalidOperationException(
                    $"'{nameof(options.RetryDelay)}' must be shorter than '{nameof(options.MessageTimeout)}' in '{nameof(EventSubscriptionOptions)}'.");

            return services
                .Configure(configure)
                .Configure<EventSubscriptionHandlerOptions>(opts =>
                {
                    var operationOptions = EventStoreClientOperationOptions.Default;

                    opts.Client = new EventStorePersistentSubscriptionsClient(new EventStoreClientSettings
                    {
                        ConnectionName = configs.ApplicationName,
                        DefaultCredentials = new UserCredentials(options.EventStoreConfigs.Username,
                            options.EventStoreConfigs.Password),
                        OperationOptions = operationOptions
                    });

                    opts.Settings = EventStore.ClientAPI.PersistentSubscriptionSettings.Create().Build();

                    //opts.Settings = new EventStore.Client.PersistentSubscriptionSettings(
                    //    startFrom: options.StartFromStreamPosition,
                    //    maxCheckPointCount: options.MaximumCheckPointCountOf,
                    //    minCheckPointCount: options.MinimumCheckPointCountOf,
                    //    checkPointAfter: TimeSpan.FromSeconds(options.CheckPointAfterSeconds),
                    //    messageTimeout: options.MessageTimeout,
                    //    resolveLinkTos: true,
                    //    maxRetryCount: options.MaxRetryCount);
                    
                    opts.StreamName = options.SubscribeToStream.StreamName;
                    opts.GroupName = options.GroupName;
                    opts.RetryDelay = options.RetryDelay;

                    if (options.SubscriptionReconnectRetryDelayInSeconds.HasValue)
                        opts.RetryReconnectDelay =
                            TimeSpan.FromSeconds(options.SubscriptionReconnectRetryDelayInSeconds.Value);

                    if (options.SubscriptionHandler is { })
                        opts.SubscriptionHandler = options.SubscriptionHandler;

                    opts.UserName = options.EventStoreConfigs.Username;
                    opts.Password = options.EventStoreConfigs.Password;
                })
                .AddHostedService<TBackgroundService>();
        }
    }
}