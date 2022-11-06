namespace SleekFlow.Todo.Application.Model.Event;

public class TodoDueDateUpdatedEventWebDto : DomainEventWebDto
{
    public DateTime? DueDate { get; set; }
}