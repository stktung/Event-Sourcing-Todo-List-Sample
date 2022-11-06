using SleekFlow.Todo.Domain.Aggregate;
using SleekFlow.Todo.Domain.Common;

namespace SleekFlow.Todo.Domain;

public interface ITodoRepository
{
    Task<long> SaveAsync(TodoAggregate todo);
    Task<TodoAggregate?> LoadLatestAsync(Guid id, long? expectedVersion = null);
    Task<IEnumerable<DomainEvent>?> GetEventsAsync(Guid id);

}