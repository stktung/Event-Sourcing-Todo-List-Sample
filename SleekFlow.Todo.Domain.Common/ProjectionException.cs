namespace SleekFlow.Todo.Domain.Common;

public class ProjectionException : Exception
{
    public ProjectionException(string? msg) : base(msg)
    {
    }
}