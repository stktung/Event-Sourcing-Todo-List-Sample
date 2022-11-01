using EventStore.ClientAPI;

namespace SleekFlow.Todo.Infrastructure.EmbeddedEventStoreDB;

public interface IEventStore
{ 
    IEventStoreConnection Connection { get; }
}