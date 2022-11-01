namespace SleekFlow.Todo.Domain;

public class TodoCreatedEvent : IEvent
{
    public Guid Id { get; set; }
    public int EventNumber { get; set; }
    public string RaisedBy { get; set; } = null!;
    public DateTime RaisedAt { get; set; }
}