namespace SleekFlow.Todo.Application.Model;

public record InsertTodoNameTextRequest(long ExpectedVersion, string Text, int Position);