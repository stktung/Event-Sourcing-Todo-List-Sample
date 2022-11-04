using SleekFlow.Todo.Domain;
using SleekFlow.Todo.Domain.Aggregate;
using SleekFlow.Todo.Infrastructure.EmbeddedEventStoreDB;
using SleekFlow.Todo.Domain.Projection;

namespace SleekFlow.Todo.Infrastructure
{
    public class TodoRepository : ITodoRepository
    {
        private const string StreamPrefix = "TodoItem-";
        private readonly IEventStore _eventStore;

        public TodoRepository(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<long> Save(TodoItemAggregate todo)
        {
            return await _eventStore.AppendAsync(BuildStreamName(todo.Id), todo.PreviousRevision, todo.NewEvents);
        }

        public async Task<TodoItemProjection?> GetAsync(Guid id)
        {
            var events = await _eventStore.ReadAllAsync(BuildStreamName(id));

            if (events == null) return null;

            var domainEvents = events.Select(esEvent => EventMapper.MapToDomainEvent(esEvent)).ToList();

            return TodoItemProjection.Load(domainEvents);
        }

        public string BuildStreamName(Guid id) => $"{StreamPrefix}-{id}";
    }
}