using Newtonsoft.Json;
using SleekFlow.Todo.Domain;
using SleekFlow.Todo.Domain.Aggregate;
using SleekFlow.Todo.Infrastructure.EmbeddedEventStoreDB;
using System.Text;
using EventStore.ClientAPI;

namespace SleekFlow.Todo.Infrastructure
{
    public class TodoRepository : ITodoRepository
    {
        public async Task Save(TodoItem todo)
        {
            var db = new EmbeddedEventStoreDb();

            using var transaction =
                await db.Connection.StartTransactionAsync($"TodoItem-{todo.Id}", todo.PreviousRevision);

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
    }
}
