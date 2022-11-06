namespace SleekFlow.Todo.Domain.Common;

public abstract class DomainEvent
{
    public DomainEvent(string eventType)
    {
        EventType = eventType;
    }

    public string EventType { get; set; }
    public Guid Id { get; set; }
    public long EventNumber { get; set; }
    public string RaisedBy { get; set; }
    public DateTime RaisedAt { get; set; }
}