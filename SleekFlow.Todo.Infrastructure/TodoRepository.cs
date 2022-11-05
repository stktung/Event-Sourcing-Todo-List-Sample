using SleekFlow.Todo.Domain;
using SleekFlow.Todo.Domain.Aggregate;
using SleekFlow.Todo.Infrastructure.EmbeddedEventStoreDb;

namespace SleekFlow.Todo.Infrastructure
{
    public class TodoRepository : ITodoRepository
    {
        private const string StreamPrefix = "Todo-";
        private readonly IEventStore _eventStore;

        public TodoRepository(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<long> Save(TodoAggregate todo)
        {
            return await _eventStore.AppendAsync(BuildStreamName(todo.Id), todo.PreviousRevision, todo.NewEvents);
        }

        public static string BuildStreamName(Guid id) => $"{StreamPrefix}-{id}";
    }
}