namespace SleekFlow.Todo.Application.Model.Event;

public class TodoDescriptionTextDeletedEventWebDto : DomainEventWebDto
{
    public int Position { get; set; }
    public int Length { get; set; }
}