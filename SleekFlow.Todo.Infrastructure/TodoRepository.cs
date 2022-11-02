using Newtonsoft.Json;
using SleekFlow.Todo.Domain;
using SleekFlow.Todo.Domain.Aggregate;
using SleekFlow.Todo.Infrastructure.EmbeddedEventStoreDB;
using System.Text;
using EventStore.ClientAPI;
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
            var domainEvents = new List<IEvent>();
            var events = await _eventStore.ReadAllAsync(BuildStreamName(id));

            if (events == null) return null;

            foreach (var esEvent in events)
            {
                switch (esEvent.Event.EventType)
                {
                    case nameof(TodoCreatedEvent):
                        var e =
                            JsonConvert.DeserializeObject<TodoCreatedEvent>(
                                Encoding.UTF8.GetString(esEvent.Event.Data));

                        e.EventNumber = esEvent.Event.EventNumber;
                        e.RaisedAt = DateTimeOffset.FromUnixTimeMilliseconds(esEvent.Event.CreatedEpoch).UtcDateTime;

                        domainEvents.Add(e);
                        break;
                }
            }

            return TodoItemProjection.Load(domainEvents);
        }

        public string BuildStreamName(Guid id) => $"{StreamPrefix}-{id}";
    }
}
