namespace SleekFlow.Todo.Domain.Event;

public class TodoCompleteUnmarkedEvent : Common.DomainEvent
{
    public TodoCompleteUnmarkedEvent() : base(nameof(TodoCompleteUnmarkedEvent))
    {
    }
}