namespace SleekFlow.Todo.Domain.Event;

public class TodoNameTextDeletedEvent : Common.DomainEvent
{
    public int Position { get; set; }
    public int Length { get; set; }
}