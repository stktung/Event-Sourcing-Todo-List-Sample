using EventStore.ClientAPI;
using EventStore.ClientAPI.Embedded;
using EventStore.Core;
using Newtonsoft.Json;
using System.Text;
using EventStore.Common.Options;
using SleekFlow.Todo.Domain;

namespace SleekFlow.Todo.Infrastructure.EmbeddedEventStoreDB
{
    public class EmbeddedEventStoreDb : IEventStore
    {
        private const int EventStoreReadStreamMaxCount = 4096;
        private static readonly TimeSpan TimeToStop = TimeSpan.FromSeconds(5);

        private readonly ClusterVNode _node;

        public IEventStoreConnection Connection { get; }
        
        public EmbeddedEventStoreDb()
        {
            var nodeBuilder = EmbeddedVNodeBuilder
                .AsSingleNode()
                .OnDefaultEndpoints()
                .RunProjections(ProjectionType.All)
                .StartStandardProjections()
                .RunInMemory();

            _node = nodeBuilder.Build();
            _node.StartAsync(true).Wait();

            Connection = EmbeddedEventStoreConnection.Create(_node);

            Connection.ConnectAsync().Wait();
        }

        public async Task<long> AppendAsync(string streamName, int expectedRevision, IEnumerable<IEvent> events)
        {
            using var transaction =
                await Connection.StartTransactionAsync(streamName, expectedRevision);

            foreach (var e in events)
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

            var result = await transaction.CommitAsync();

            return result.NextExpectedVersion;
        }

        public async Task<IEnumerable<ResolvedEvent>?> ReadAllAsync(string streamName)
        {
            var slice =
                await Connection.ReadStreamEventsForwardAsync(streamName, StreamPosition.Start,
                    EventStoreReadStreamMaxCount, true);

            if (slice.Status == SliceReadStatus.StreamNotFound)
                return null;

            return slice.Events;
        }

        public void Dispose()
        {
            Connection?.Close();
            _node.StopAsync(TimeToStop).Wait();
        }
    }
}