namespace SleekFlow.Todo.Domain.Event;

public class TodoDescriptionTextInsertedEvent : Common.DomainEvent
{
    public string Text { get; set; } = string.Empty;
    public int Position { get; set; }

    public TodoDescriptionTextInsertedEvent() : base(nameof(TodoDescriptionTextInsertedEvent))
    {
    }
}