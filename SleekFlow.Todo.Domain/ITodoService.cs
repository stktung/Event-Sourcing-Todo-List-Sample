using SleekFlow.Todo.Domain.Projection;

namespace SleekFlow.Todo.Domain;

public interface ITodoService
{
    Task<Guid> CreateTodoAsync();
    Task<TodoItemProjection?> GetAsync(Guid id);
}