namespace SleekFlow.Todo.Application.EventSubscription;

public class EventStoreConfigs
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string ApplicationName { get; set; } = "";
}

public class EventStream
{
    public string StreamName { get; }
    public EventStream(string streamName) =>
        StreamName = streamName;
}

public abstract class SystemProjectionEventStream : EventStream
{
    protected SystemProjectionEventStream(string name, string type)
        : base($"{type}-{name}")
    { }
}

public class CategoryStream : SystemProjectionEventStream
{
    public CategoryStream(string name)
        : base(name, "$ce")
    { }
}