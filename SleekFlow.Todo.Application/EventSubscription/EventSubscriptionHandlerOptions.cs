using EventStore.Client;
using SleekFlow.Todo.Domain;

namespace SleekFlow.Todo.Application.EventSubscription;

public class EventSubscriptionHandlerOptions
{
    public EventStorePersistentSubscriptionsClient Client { get; set; } = null!;
    public EventStore.ClientAPI.PersistentSubscriptionSettings Settings { get; set; } = null!;
    public string StreamName { get; set; } = "";
    public TimeSpan RetryReconnectDelay { get; set; } = TimeSpan.FromSeconds(30);
    public TimeSpan RetryDelay { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string GroupName { get; set; }
}