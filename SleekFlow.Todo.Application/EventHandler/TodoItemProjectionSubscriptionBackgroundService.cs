using Microsoft.Extensions.Options;
using SleekFlow.Todo.Application.EventSubscription;
using SleekFlow.Todo.Domain;
using SleekFlow.Todo.Infrastructure.EmbeddedEventStoreDB;

namespace SleekFlow.Todo.Application.EventHandler;

public class TodoItemProjectionEventHandler : ITodoItemProjectionEventHandler
{
    public HandlerResult HandleEvent(IEvent evt)
    {
        Console.WriteLine("Hi Dare");
        return new HandlerResult() { Error = Errors.RetryEvent<TodoCreatedEvent>("Testing") };
    }
}

public interface ITodoItemProjectionEventHandler
{
    HandlerResult HandleEvent(IEvent evt);
}

public class TodoItemProjectionSubscriptionBackgroundService : EventSubscriptionBackgroundService
{
    public TodoItemProjectionSubscriptionBackgroundService(ILogger<EventSubscriptionBackgroundService> logger,
        IOptions<EventSubscriptionHandlerOptions> options, IEventStore eventStore, ITodoItemProjectionEventHandler handler) : base(logger,
        options, eventStore)
    {
        _subscriptionHandler = handler.HandleEvent;
    }
}