namespace SleekFlow.Todo.Application.Model;

public record UpdateTodoDueDateRequest(long ExpectedVersion, DateTime? DueDate);