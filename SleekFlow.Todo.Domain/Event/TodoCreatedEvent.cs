namespace SleekFlow.Todo.Domain.Event;

public class TodoCreatedEvent : Common.DomainEvent
{
    public TodoCreatedEvent() : base(nameof(TodoCreatedEvent))
    {
    }
}