using SleekFlow.Todo.Domain.Aggregate;

namespace SleekFlow.Todo.Domain;

public interface ITodoRepository
{
    Task<long> SaveAsync(TodoAggregate todo);
    Task<TodoAggregate?> LoadLatestAsync(Guid id, long? expectedVersion = null);
}