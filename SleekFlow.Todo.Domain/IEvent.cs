namespace SleekFlow.Todo.Domain;

public interface IEvent
{
    int EventNumber { get; set; }
    string RaisedBy { get; set; }
    DateTime RaisedAt { get; set; }
}