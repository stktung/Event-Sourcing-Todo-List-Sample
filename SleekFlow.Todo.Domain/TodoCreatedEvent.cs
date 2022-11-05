using SleekFlow.Todo.Domain.Common;

namespace SleekFlow.Todo.Domain;

public class TodoCreatedEvent : IEvent
{
    public Guid Id { get; set; }
    public long EventNumber { get; set; }
    public string RaisedBy { get; set; } = null!;
    public DateTime RaisedAt { get; set; }
}