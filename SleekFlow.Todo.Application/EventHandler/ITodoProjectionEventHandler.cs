using SleekFlow.Todo.Domain.Common;
using SleekFlow.Todo.Infrastructure.EventSubscription;

namespace SleekFlow.Todo.Application.EventHandler;

public interface ITodoProjectionEventHandler
{
    HandlerResult HandleEvent(IEvent evt);
}