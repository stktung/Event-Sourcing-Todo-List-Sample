using EventStore.ClientAPI;
using SleekFlow.Todo.Domain;

namespace SleekFlow.Todo.Infrastructure.EmbeddedEventStoreDB;

public interface IEventStore
{ 
    IEventStoreConnection Connection { get; }
    Task<long> AppendAsync(string streamName, int expectedRevision, IEnumerable<IEvent> events);
    Task<IEnumerable<ResolvedEvent>> ReadAllAsync(string streamName);
}