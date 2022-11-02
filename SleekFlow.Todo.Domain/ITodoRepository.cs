using SleekFlow.Todo.Domain.Aggregate;
using SleekFlow.Todo.Domain.Projection;

namespace SleekFlow.Todo.Domain;

public interface ITodoRepository
{
    Task<long> Save(TodoItemAggregate todo);
    Task<TodoItemProjection?> GetAsync(Guid id);
}