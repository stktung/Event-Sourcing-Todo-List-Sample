namespace SleekFlow.Todo.Application.Model;

public record UpdateTodoIsCompletedRequest(long ExpectedVersion, bool IsCompleted);