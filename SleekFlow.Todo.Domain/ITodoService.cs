using SleekFlow.Todo.Domain.Projection;

namespace SleekFlow.Todo.Domain;

public interface ITodoService
{
    Task<(Guid Id, long LastEventNumber)> CreateTodoAsync();
    Task<TodoItemProjection?> GetAsync(Guid id);
}