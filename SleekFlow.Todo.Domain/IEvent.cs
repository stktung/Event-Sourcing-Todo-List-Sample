namespace SleekFlow.Todo.Domain;

public interface IEvent
{
    long EventNumber { get; set; }
    string RaisedBy { get; set; }
    DateTime RaisedAt { get; set; }
}