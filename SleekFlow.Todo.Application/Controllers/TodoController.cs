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
        public async Task<GetResponse> GetAsync([FromRoute]Guid id)
        {
            return _mapper.Map<GetResponse>(await _service.GetAsync(id));
        }
    }

    public record CreateTodoResponse(Guid Id);

    public record GetResponse(Guid Id);
}