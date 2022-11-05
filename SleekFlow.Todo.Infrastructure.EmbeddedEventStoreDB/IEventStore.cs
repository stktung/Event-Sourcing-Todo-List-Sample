using EventStore.ClientAPI;
using SleekFlow.Todo.Domain;
using SleekFlow.Todo.Domain.Common;

namespace SleekFlow.Todo.Infrastructure.EmbeddedEventStoreDb;

public interface IEventStore
{ 
    IEventStoreConnection Connection { get; }
    Task<long> AppendAsync(string streamName, int expectedRevision, IEnumerable<IEvent> events);
    Task<IEnumerable<ResolvedEvent>> ReadAllAsync(string streamName);
}