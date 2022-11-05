using SleekFlow.Todo.Domain.Projection;

namespace SleekFlow.Todo.Domain;

public interface ITodoProjectionRepository
{
    Task<TodoProjection?> GetFromEventStoreAsync(Guid id);
    Task<IEnumerable<TodoProjection>?> GetAllAsync();
    Task Save(Guid id);
}