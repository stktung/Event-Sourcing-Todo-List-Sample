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

        public async Task Save(TodoItemAggregate todo)
        {
            await _eventStore.AppendAsync(BuildStreamName(todo.Id), todo.PreviousRevision, todo.NewEvents);
        }

        public async Task<TodoItemProjection?> GetAsync(Guid id)
        {
            var domainEvents = new List<IEvent>();
            foreach (var esEvent in await _eventStore.ReadAllAsync(BuildStreamName(id)))
            {
                switch (esEvent.Event.EventType)
                {
                    case nameof(TodoCreatedEvent):
                        var e =
                            JsonConvert.DeserializeObject<TodoCreatedEvent>(
                                Encoding.UTF8.GetString(esEvent.Event.Data));

                        e.EventNumber = esEvent.Event.EventNumber;
                        
                        domainEvents.Add(e);
                        break;
                }
            }

            return TodoItemProjection.Load(domainEvents);
        }

        public string BuildStreamName(Guid id) => $"{StreamPrefix}-{id}";
    }
}
