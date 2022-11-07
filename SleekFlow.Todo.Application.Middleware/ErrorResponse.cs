namespace SleekFlow.Todo.Application.Middleware;

/// <summary>
/// Generic error response
/// </summary>
/// <param name="ErrorType">The exception type. Currently includes 'DomainException', 'ProjectionException', 'AggregateWrongExpectedVersionException'</param>
/// <param name="Message"></param>
/// <param name="Exception"></param>
public record ErrorResponse(string ErrorType, string Message, string Exception);