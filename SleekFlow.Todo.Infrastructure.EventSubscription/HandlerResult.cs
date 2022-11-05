namespace SleekFlow.Todo.Infrastructure.EventSubscription;

public class HandlerResult
{
    public bool IsSuccess;
    public Error? Error;
}

public class Error
{
    public string Code;
    public string Message;

    public Error(string code, string message)
    {
        Code = code;
        Message = message;
    }
}

public static class Errors
{
    public static Error InvalidEventLink(string link)
        => new Error(nameof(InvalidEventLink), $"Invalid event link: '{link}'");

    public static Error SkipEvent<T>(string reason)
        => new Error(nameof(SkipEvent), $"Event '{typeof(T).Name}' should be skipped. Reason: '{reason}'.");

    public static Error RetryEvent<T>(string reason)
        => new Error(nameof(RetryEvent), $"Event '{typeof(T).Name}' is to be retried. Reason: '{reason}'.");

    public static Error ParkEvent<T>(string reason)
        => new Error(nameof(ParkEvent), $"Event '{typeof(T).Name}' should be parked. Reason: '{reason}'.");

}