using SleekFlow.Todo.Domain;
using SleekFlow.Todo.Domain.Common;
using SleekFlow.Todo.Domain.Event;
using SleekFlow.Todo.Infrastructure.EventSubscription;

namespace SleekFlow.Todo.Application.EventHandler;

public class TodoProjectionEventHandler : ITodoProjectionEventHandler
{
    private readonly ITodoProjectionRepository _repository;

    public TodoProjectionEventHandler(ITodoProjectionRepository repository)
    {
        _repository = repository;
    }

    public HandlerResult HandleEvent(DomainEvent evt)
    {
        try
        {
            switch (evt)
            {
                case TodoCreatedEvent:
                case TodoNameTextInsertedEvent:
                    _repository.Save(evt.Id);
                    break;
            }
        }
        catch (Exception e)
        {
            return new HandlerResult
                { Error = Errors.ParkEvent<DomainEvent>($"Unhandled Exception: {e}") };
        }

        return new HandlerResult { IsSuccess = true };
    }
}