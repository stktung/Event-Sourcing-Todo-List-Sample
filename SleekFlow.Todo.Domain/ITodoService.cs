using SleekFlow.Todo.Domain.Aggregate;

namespace SleekFlow.Todo.Domain;

public interface ITodoService
{
    Task<(Guid Id, long LastEventNumber)> CreateTodoAsync();

    Task<(TodoAggregate? Todo, long LastEventNumber)> InsertTodoNameTextAsync(long expectedVersion, Guid id,
        string text, int position);
}