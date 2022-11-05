using Microsoft.Extensions.Options;
using SleekFlow.Todo.Infrastructure.EmbeddedEventStoreDb;
using SleekFlow.Todo.Infrastructure.EventSubscription;

namespace SleekFlow.Todo.Application.EventHandler;

public class TodoProjectionSubscriptionBackgroundService : EventSubscriptionBackgroundService
{
    public TodoProjectionSubscriptionBackgroundService(ILogger<EventSubscriptionBackgroundService> logger,
        IOptions<EventSubscriptionHandlerOptions> options, IEventStore eventStore, ITodoProjectionEventHandler handler) : base(logger,
        options, eventStore)
    {
        SubscriptionHandler = handler.HandleEvent;
    }
}