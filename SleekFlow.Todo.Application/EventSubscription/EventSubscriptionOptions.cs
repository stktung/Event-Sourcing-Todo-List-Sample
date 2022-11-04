using EventStore.Client;
using SleekFlow.Todo.Domain;

namespace SleekFlow.Todo.Application.EventSubscription;

public class EventSubscriptionOptions
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string ApplicationName { get; set; } = "";
    public string SubscribeToStream { get; set; } = null!;
}