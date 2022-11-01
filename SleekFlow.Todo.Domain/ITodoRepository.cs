using SleekFlow.Todo.Domain.Aggregate;

namespace SleekFlow.Todo.Domain;

public interface ITodoRepository
{
    Task Save(TodoItem todo);
    Task<TodoItem?> GetAsync(Guid id);
}