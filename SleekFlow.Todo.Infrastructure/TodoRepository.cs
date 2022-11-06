using EventStore.ClientAPI.Exceptions;
using SleekFlow.Todo.Domain;
using SleekFlow.Todo.Domain.Aggregate;
using SleekFlow.Todo.Domain.Common;
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

        public async Task<IEnumerable<DomainEvent>?> GetEventsAsync(Guid id)
        {
            var events = await _eventStore.ReadAllAsync(BuildStreamName(id));

            if (events == null) return null;

            return events.Select(EventMapper.MapToDomainEvent).ToList();
        }

        public async Task<TodoAggregate?> LoadLatestAsync(Guid id, long? expectedVersion = null)
        {
            var events = await _eventStore.ReadAllAsync(BuildStreamName(id));

            if (events == null) return null;

            var domainEvents = events.Select(EventMapper.MapToDomainEvent).ToList();

            var todo = TodoAggregate.Load(domainEvents);

            if (expectedVersion != null && todo.LoadVersion != expectedVersion)
                throw new AggregateWrongExpectedVersionException(
                    $"Revision of loaded aggregate is mismatched with expected version. LoadVersion: '{todo.LoadVersion}' ExpectedVersion '{expectedVersion}'");

            return todo;
        }

        public async Task<long> SaveAsync(TodoAggregate todo)
        {
            try
            {
                return await _eventStore.AppendAsync(BuildStreamName(todo.Id), todo.LoadVersion, todo.NewEvents);
            }
            catch (WrongExpectedVersionException e)
            {
                throw new AggregateWrongExpectedVersionException(
                    $"Revision of aggregate is mismatched with expected version. ExpectedVersion: {todo.LoadVersion}",
                    e);
            }
        }

        public static string BuildStreamName(Guid id) => $"{StreamPrefix}-{id}";
    }
}