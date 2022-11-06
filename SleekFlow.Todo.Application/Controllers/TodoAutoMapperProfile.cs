using AutoMapper;
using SleekFlow.Todo.Application.Model;
using SleekFlow.Todo.Application.Model.Event;
using SleekFlow.Todo.Domain.Common;
using SleekFlow.Todo.Domain.Event;
using SleekFlow.Todo.Domain.Projection;

namespace SleekFlow.Todo.Application.Controllers;

public class TodoAutoMapperProfile : Profile
{
    public TodoAutoMapperProfile()
    {
        CreateMap<TodoProjection, GetTodoResponse>();
        CreateMap<DomainEvent, DomainEventWebDto>()
            .Include<TodoCreatedEvent, TodoCreatedEventWebDto>()
            .Include<TodoNameTextInsertedEvent, TodoNameTextInsertedEventWebDto>()
            .Include<TodoNameTextDeletedEvent, TodoNameTextDeletedEventWebDto>()
            .Include<TodoDescriptionTextInsertedEvent, TodoDescriptionTextInsertedEventWebDto>()
            .Include<TodoDescriptionTextDeletedEvent, TodoDescriptionTextDeletedEventWebDto>()
            .Include<TodoDueDateUpdatedEvent, TodoDueDateUpdatedEventWebDto>()
            .Include<TodoCompleteMarkedEvent, TodoCompleteMarkedEventWebDto>()
            .Include<TodoCompleteUnmarkedEvent, TodoCompleteUnmarkedEventWebDto>();

        CreateMap<TodoCreatedEvent, TodoCreatedEventWebDto>();
        CreateMap<TodoNameTextInsertedEvent, TodoNameTextInsertedEventWebDto>();
        CreateMap<TodoNameTextDeletedEvent, TodoNameTextDeletedEventWebDto>();
        CreateMap<TodoDescriptionTextInsertedEvent, TodoDescriptionTextInsertedEventWebDto>();
        CreateMap<TodoDescriptionTextDeletedEvent, TodoDescriptionTextDeletedEventWebDto>();
        CreateMap<TodoDueDateUpdatedEvent, TodoDueDateUpdatedEventWebDto>();
        CreateMap<TodoCompleteMarkedEvent, TodoCompleteMarkedEventWebDto>();
        CreateMap<TodoCompleteUnmarkedEvent, TodoCompleteUnmarkedEventWebDto>(); ;
    }
}