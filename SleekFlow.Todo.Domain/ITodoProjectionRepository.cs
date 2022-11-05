using SleekFlow.Todo.Domain.Projection;

namespace SleekFlow.Todo.Domain;

public interface ITodoProjectionRepository
{
    Task<TodoProjection?> GetFromEventStoreAsync(Guid id);
    Task<IEnumerable<TodoProjection>?> GetAll();
    Task Save(Guid id);
}