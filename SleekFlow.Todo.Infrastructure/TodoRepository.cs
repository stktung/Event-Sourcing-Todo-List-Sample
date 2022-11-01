using Newtonsoft.Json;
using SleekFlow.Todo.Domain;
using SleekFlow.Todo.Domain.Aggregate;
using SleekFlow.Todo.Infrastructure.EmbeddedEventStoreDB;
using System.Text;
using EventStore.ClientAPI;
using static EventStore.Core.Services.Storage.ReaderIndex.EventFilter;
using System.Data.Common;

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

        public async Task Save(TodoItem todo)
        {
            var db = new EmbeddedEventStoreDb();

            using var transaction =
                await db.Connection.StartTransactionAsync(BuildStreamName(todo.Id), todo.PreviousRevision);

            foreach (var e in todo.NewEvents)
            {
                var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(e));
                var metadata = Encoding.UTF8.GetBytes("{}");

                var eventPayload = new EventData(
                    Guid.NewGuid(),
                    e.GetType().Name,
                    true,
                    data,
                    metadata);


                await transaction.WriteAsync(eventPayload);
            }

            await transaction.CommitAsync();
        }

        public async Task<TodoItem?> GetAsync(Guid id)
        {
            var slice =
                await _eventStore.Connection.ReadStreamEventsForwardAsync(BuildStreamName(id), StreamPosition.Start,
                    int.MaxValue, true);

            if (slice.Status == SliceReadStatus.StreamNotFound)
                return null;

            var domainEvents = new List<IEvent>();
            foreach (var esEvent in slice.Events)
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

            return TodoItem.Load(domainEvents);
        }

        public static string BuildStreamName(Guid id) => $"{StreamPrefix}{id}";
    }
}
