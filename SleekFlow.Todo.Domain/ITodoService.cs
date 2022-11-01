using SleekFlow.Todo.Domain.Aggregate;

namespace SleekFlow.Todo.Domain;

public interface ITodoService
{
    Task<Guid> CreateTodoAsync();
    Task<TodoItem?> GetAsync(Guid id);
}