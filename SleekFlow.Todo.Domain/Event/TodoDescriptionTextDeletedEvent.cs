namespace SleekFlow.Todo.Domain.Event;

public class TodoDescriptionTextDeletedEvent : Common.DomainEvent
{
    public int Position { get; set; }
    public int Length { get; set; }
}