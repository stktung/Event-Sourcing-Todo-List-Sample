namespace SleekFlow.Todo.Application.Middleware;

public record ErrorResponse(string ErrorType, string Message, string Exception);