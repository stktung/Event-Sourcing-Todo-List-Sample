using SleekFlow.Todo.Domain.Projection;

namespace SleekFlow.Todo.Domain;

public interface ITodoProjectionRepository
{
    Task<TodoProjection?> GetFromEventStoreAsync(Guid id);

    Task<IEnumerable<TodoProjection>?> GetAllAsync(bool? isCompleted = null, DateTime? dueDateIsBefore = null,
        DateTime? dueDateIsAfter = null, SortByField? sortByField = null, bool? sortByAsc = null);
    Task Save(Guid id);
}
public enum SortByField
{
    Name,
    DueDate
}
