namespace SleekFlow.Todo.Domain.Event;

public class TodoNameTextInsertedEvent : Common.DomainEvent
{
    public string Text { get; set; } = string.Empty;
    public int Position { get; set; }
}

public class TodoNameTextDeletedEvent : Common.DomainEvent
{
    public int Position { get; set; }
    public int Length { get; set; }
}