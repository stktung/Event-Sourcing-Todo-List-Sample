using EventStore.Client;
using SleekFlow.Todo.Domain;

namespace SleekFlow.Todo.Application.EventSubscription;

public class EventSubscriptionOptions
{
    public EventStoreConfigs EventStoreConfigs { get; set; } = null!;
    /// <summary>
    /// The stream that the events come from.
    /// </summary>
    public EventStream SubscribeToStream { get; set; } = null!;
    public int MaximumCheckPointCountOf { get; set; } = 1;
    public int MinimumCheckPointCountOf { get; set; } = 1;
    public int CheckPointAfterSeconds { get; set; } = 10;
    public int? SubscriptionReconnectRetryDelayInSeconds { get; set; }
    /// <summary>
    /// Which event position in the stream the subscription should start from (default <see cref="EventStore.ClientAPI.StreamPosition.Start"/>).
    /// </summary>
    public StreamPosition StartFromStreamPosition { get; set; } = StreamPosition.Start;
    /// <summary>
    /// The amount of time after which to consider a message as timed out and retried.
    /// </summary>
    public TimeSpan MessageTimeout { get; set; } = TimeSpan.FromSeconds(30);
    /// <summary>
    /// The maximum number of retries (due to timeout) before a message is considered to be parked (default 100).
    /// </summary>
    public int MaxRetryCount { get; set; } = 100;
    /// <summary>
    /// The amount of time before a message is acknowledged as retry (default 1 second).
    /// </summary>
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);
    public Func<IEvent, HandlerResult>? SubscriptionHandler { get; set; }
    public string GroupName { get; set; }
}