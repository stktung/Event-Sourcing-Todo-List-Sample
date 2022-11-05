using EventStore.ClientAPI;
using EventStore.ClientAPI.Embedded;
using EventStore.Core;
using Newtonsoft.Json;
using System.Text;
using EventStore.Common.Options;
using SleekFlow.Todo.Domain;
using SleekFlow.Todo.Domain.Common;

namespace SleekFlow.Todo.Infrastructure.EmbeddedEventStoreDb
{
    public class EmbeddedEventStoreDb : IEventStore, IDisposable
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

        /// <summary>
        /// Appends one or more events into stream. If stream is not on the expected revision, exception will be thrown
        /// </summary>
        /// <param name="streamName">Name of stream to append into the event store</param>
        /// <param name="expectedRevision">Append expects the stream to be on the expected revision. Otherwise exception will be thrown</param>
        /// <param name="events">Collection of domain events to insert into the event store</param>
        /// <returns>Next Event Number</returns>
        public async Task<long> AppendAsync(string streamName, int expectedRevision, IEnumerable<DomainEvent> events)
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