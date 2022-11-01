using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SleekFlow.Todo.Domain;

namespace SleekFlow.Todo.Application.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _service;
        private readonly IMapper _mapper;

        public TodoController(ITodoService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpPost("create")]
        public async Task<CreateTodoResponse> CreateTodoAsync()
        {
            return new CreateTodoResponse(await _service.CreateTodoAsync());
        }

        [Route("{id:guid}")]
        [HttpGet]
        public async Task<IActionResult> GetAsync([FromRoute]Guid id)
        {
            var todo = await _service.GetAsync(id);
            if (todo == null) return NotFound();

            return Ok(_mapper.Map<GetTodoResponse>(todo));
        }
    }

    public record CreateTodoResponse(Guid Id);

    public record GetTodoResponse(Guid Id);
}