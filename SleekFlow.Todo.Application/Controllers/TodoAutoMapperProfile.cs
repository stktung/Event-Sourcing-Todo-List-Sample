using AutoMapper;
using SleekFlow.Todo.Domain.Projection;

namespace SleekFlow.Todo.Application.Controllers;

public class TodoAutoMapperProfile : Profile
{
    public TodoAutoMapperProfile()
    {
        CreateMap<TodoItemProjection, GetTodoResponse>();
    }
}