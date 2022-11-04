namespace SleekFlow.Todo.Infrastructure.EventSubscription;

public class EventSubscriptionOptions
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string ApplicationName { get; set; } = "";
    public string SubscribeToStream { get; set; } = null!;
}