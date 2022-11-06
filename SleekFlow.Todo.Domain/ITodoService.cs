using SleekFlow.Todo.Domain.Aggregate;

namespace SleekFlow.Todo.Domain;

public interface ITodoService
{
    Task<(Guid Id, long LastEventNumber)> CreateTodoAsync();

    Task<(TodoAggregate todo, long lastEventNumber)> InsertTodoNameTextAsync(long expectedVersion, Guid id,
        string text, int position);

    Task<(TodoAggregate todo, long lastEventNumber)> DeleteTodoNameTextAsync(long expectedVersion, Guid id,
        int position, int length);
}