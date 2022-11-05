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
        private readonly ITodoProjectionRepository _projectionRepository;
        private readonly IMapper _mapper;

        public TodoController(ITodoService service, ITodoProjectionRepository projectionRepository, IMapper mapper)
        {
            _service = service;
            _projectionRepository = projectionRepository;
            _mapper = mapper;
        }
        
        [HttpPost("create")]
        public async Task<CreateTodoResponse> CreateTodoAsync()
        {
            var result = await _service.CreateTodoAsync();

            return new CreateTodoResponse(result.Id, result.LastEventNumber);
        }

        [Route("{id:guid}")]
        [HttpGet]
        public async Task<IActionResult> GetAsync([FromRoute]Guid id)
        {
            var todo = await _projectionRepository.GetFromEventStoreAsync(id);
            if (todo == null) return NotFound();

            return Ok(_mapper.Map<GetTodoResponse>(todo));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var todos = await _projectionRepository.GetAllAsync();

            if (todos == null || !todos.Any()) return NotFound();

            return Ok(todos.Select(todo => _mapper.Map<GetTodoResponse>(todo)));
        }

    }

    public record CreateTodoResponse(Guid Id, long LastEventNumber);

    public record GetTodoResponse(Guid Id, string? Name, string? Description, DateTime? DueDate, bool Completed,
        DateTime LastUpdatedAt, long LastEventNumber);

}