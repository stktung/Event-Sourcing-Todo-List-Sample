namespace SleekFlow.Todo.Domain.Common;

public class AggregateWrongExpectedVersionException : Exception
{
    public AggregateWrongExpectedVersionException(string? msg) : base(msg)
    {
    }

    public AggregateWrongExpectedVersionException(string? message, Exception? innerException) : base(message,
        innerException)
    {
    }
}