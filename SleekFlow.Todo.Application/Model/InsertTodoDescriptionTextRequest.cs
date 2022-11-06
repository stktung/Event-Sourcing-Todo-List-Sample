namespace SleekFlow.Todo.Application.Model;

public record InsertTodoDescriptionTextRequest(long ExpectedVersion, string Text, int Position);