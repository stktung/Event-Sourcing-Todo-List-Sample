namespace SleekFlow.Todo.Domain.Event;

public class TodoCompleteMarkedEvent : Common.DomainEvent
{
    public TodoCompleteMarkedEvent() : base(nameof(TodoCompleteMarkedEvent))
    {
    }
}