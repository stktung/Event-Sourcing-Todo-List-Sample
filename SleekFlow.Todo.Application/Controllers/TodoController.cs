using Microsoft.AspNetCore.Mvc;
using SleekFlow.Todo.Domain;
using SleekFlow.Todo.Domain.Aggregate;

namespace SleekFlow.Todo.Application.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _service;

        public TodoController(ITodoService service)
        {
            _service = service;
        }
        
        [HttpPost]
        public async Task<CreateTodoResponse> CreateTodoAsync()
        {
            return new CreateTodoResponse(await _service.CreateTodoAsync());
        }

        [HttpGet]
        public async Task<TodoItem> GetAsync(Guid id)
        {
            return await _service.GetAsync(id);
        }

    }

    public record CreateTodoResponse(Guid Id);
}