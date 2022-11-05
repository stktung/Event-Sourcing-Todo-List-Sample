namespace SleekFlow.Todo.Domain.Common;

public interface IEvent
{
    long EventNumber { get; set; }
    string RaisedBy { get; set; }
    DateTime RaisedAt { get; set; }
}