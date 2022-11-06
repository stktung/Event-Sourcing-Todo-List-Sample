using EventStore.ClientAPI;
using SleekFlow.Todo.Domain.Common;

namespace SleekFlow.Todo.Infrastructure.EmbeddedEventStoreDb;

public interface IEventStore
{ 
    IEventStoreConnection Connection { get; }
    Task<long> AppendAsync(string streamName, long expectedRevision, IEnumerable<DomainEvent> events);
    Task<IEnumerable<ResolvedEvent>?> ReadAllAsync(string streamName);
}