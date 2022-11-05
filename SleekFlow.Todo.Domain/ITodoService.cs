namespace SleekFlow.Todo.Domain;

public interface ITodoService
{
    Task<(Guid Id, long LastEventNumber)> CreateTodoAsync();
}