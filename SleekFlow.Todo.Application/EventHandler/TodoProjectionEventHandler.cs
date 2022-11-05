using SleekFlow.Todo.Domain;
using SleekFlow.Todo.Domain.Common;
using SleekFlow.Todo.Infrastructure.EventSubscription;

namespace SleekFlow.Todo.Application.EventHandler;

public class TodoProjectionEventHandler : ITodoProjectionEventHandler
{
    private readonly ITodoProjectionRepository _repository;

    public TodoProjectionEventHandler(ITodoProjectionRepository repository)
    {
        _repository = repository;
    }

    public HandlerResult HandleEvent(IEvent evt)
    {
        switch (evt)
        {
            case TodoCreatedEvent todoCreatedEvent:
                _repository.Save(todoCreatedEvent.Id);
                break;
            default:
                return new HandlerResult
                    { Error = Errors.ParkEvent<IEvent>($"Can not handle event of type '{evt.GetType()}'") };
        }

        return new HandlerResult { IsSuccess = true };
    }
}