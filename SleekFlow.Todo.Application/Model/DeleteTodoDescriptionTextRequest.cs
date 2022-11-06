namespace SleekFlow.Todo.Application.Model;

public record DeleteTodoDescriptionTextRequest(long ExpectedVersion, int Position, int length);