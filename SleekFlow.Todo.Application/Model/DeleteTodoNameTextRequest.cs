namespace SleekFlow.Todo.Application.Model;

public record DeleteTodoNameTextRequest(long ExpectedVersion, int Position, int length);