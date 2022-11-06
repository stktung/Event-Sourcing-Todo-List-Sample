namespace SleekFlow.Todo.Application.Model.Event;

public class TodoDescriptionTextInsertedEventWebDto : DomainEventWebDto
{
    public string Text { get; set; } = string.Empty;
    public int Position { get; set; }
}