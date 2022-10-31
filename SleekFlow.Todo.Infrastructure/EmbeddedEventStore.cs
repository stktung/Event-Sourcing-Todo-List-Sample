using EventStore.ClientAPI;
using EventStore.ClientAPI.Embedded;
using EventStore.Core;
using Microsoft.Diagnostics.Tracing.Parsers.Tpl;
using System.Diagnostics;

namespace SleekFlow.Todo.Infrastructure
{
    public class EmbeddedEventStore
    {
        private static readonly TimeSpan TimeToStop = TimeSpan.FromSeconds(5);

        private ClusterVNode _node;

        public EmbeddedEventStore()
        {
            var nodeBuilder = EmbeddedVNodeBuilder
                .AsSingleNode()
                .OnDefaultEndpoints()
                .RunInMemory();

            _node = nodeBuilder.Build();
            _node.StartAsync(true).Wait();

            Connection = EmbeddedEventStoreConnection.Create(_node);
        }

        public IEventStoreConnection Connection { get; }

        public void Dispose()
        {
            Connection?.Close();
            _node.StopAsync(TimeToStop).Wait();
        }
    }
}