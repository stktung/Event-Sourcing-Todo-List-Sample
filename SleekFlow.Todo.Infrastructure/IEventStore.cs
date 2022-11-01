using EventStore.ClientAPI;

namespace SleekFlow.Todo.Infrastructure;

public interface IEventStore
{ 
    IEventStoreConnection Connection { get; }
}