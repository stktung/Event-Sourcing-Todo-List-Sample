using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SleekFlow.Todo.Application.Model;
using SleekFlow.Todo.Application.Model.Event;
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

        [Route("{id:guid}")]
        [HttpGet]
        public async Task<IActionResult> GetAsync([FromRoute] Guid id)
        {
            var todo = await _projectionRepository.GetFromEventStoreAsync(id);
            if (todo == null) return NotFound();

            return Ok(_mapper.Map<GetTodoResponse>(todo));
        }

        [Route("{id:guid}/history")]
        [HttpGet]
        public async Task<IActionResult> GetHistoryAsync([FromRoute] Guid id)
        {
            var events = await _service.GetHistory(id);
            if (events == null) return NotFound();

            return Ok(events.Select(e => _mapper.Map(e, e.GetType(), typeof(DomainEventWebDto))));
        }


        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] bool? isCompleted = null,
            [FromQuery] DateTime? dueDateIsBefore = null, [FromQuery] DateTime? dueDateIsAfter = null,
            [FromQuery] SortByField? sortByField = null, [FromQuery] bool? sortByAsc = null)
        {
            var todos = await _projectionRepository.GetAllAsync(isCompleted, dueDateIsBefore, dueDateIsAfter,
                (Domain.SortByField?)sortByField, sortByAsc);

            if (todos == null || !todos.Any()) return NotFound();

            return Ok(todos.Select(todo => _mapper.Map<GetTodoResponse>(todo)));
        }

        public enum SortByField
        {
            Name,
            DueDate
        }

        [HttpPost("create")]
        public async Task<GeneralPostTodoResponse> CreateTodoAsync()
        {
            var result = await _service.CreateTodoAsync();

            return new GeneralPostTodoResponse(result.Id, result.LastEventNumber);
        }

        [Route("{Id:guid}/name/inserttext")]
        [HttpPost]
        public async Task<IActionResult> InsertTodoNameTextAsync([FromRoute] Guid Id, [FromBody] InsertTodoNameTextRequest request)
        {
            var (todo, lastEventNumber) = await _service.InsertTodoNameTextAsync(request.ExpectedVersion, Id,
                request.Text, request.Position);

            if (todo == null) return NotFound();

            return Ok(new GeneralPostTodoResponse(todo.Id, lastEventNumber));
        }

        [Route("{Id:guid}/name/deletetext")]
        [HttpPost]
        public async Task<IActionResult> DeleteTodoNameTextAsync([FromRoute] Guid Id, [FromBody] DeleteTodoNameTextRequest request)
        {
            var (todo, lastEventNumber) = await _service.DeleteTodoNameTextAsync(request.ExpectedVersion, Id,
                request.Position, request.length);

            if (todo == null) return NotFound();

            return Ok(new GeneralPostTodoResponse(todo.Id, lastEventNumber));
        }

        [Route("{Id:guid}/description/inserttext")]
        [HttpPost]
        public async Task<IActionResult> InsertTodoDescriptionTextAsync([FromRoute] Guid Id, [FromBody] InsertTodoDescriptionTextRequest request)
        {
            var (todo, lastEventNumber) = await _service.InsertTodoDescriptionTextAsync(request.ExpectedVersion, Id,
                request.Text, request.Position);

            if (todo == null) return NotFound();

            return Ok(new GeneralPostTodoResponse(todo.Id, lastEventNumber));
        }

        [Route("{Id:guid}/description/deletetext")]
        [HttpPost]
        public async Task<IActionResult> DeleteTodoDescriptionTextAsync([FromRoute] Guid Id, [FromBody] DeleteTodoDescriptionTextRequest request)
        {
            var (todo, lastEventNumber) = await _service.DeleteTodoDescriptionTextAsync(request.ExpectedVersion, Id,
                request.Position, request.length);

            if (todo == null) return NotFound();

            return Ok(new GeneralPostTodoResponse(todo.Id, lastEventNumber));
        }

        [Route("{Id:guid}/duedate")]
        [HttpPut]
        public async Task<IActionResult> UpdateTodoDueDateAsync([FromRoute] Guid Id, [FromBody] UpdateTodoDueDateRequest request)
        {
            var (todo, lastEventNumber) = await _service.UpdateTodoDueDateAsync(request.ExpectedVersion, Id,
                request.DueDate);

            if (todo == null) return NotFound();

            return Ok(new GeneralPostTodoResponse(todo.Id, lastEventNumber));
        }

        [Route("{Id:guid}/completed")]
        [HttpPut]
        public async Task<IActionResult> UpdateTodoIsCompleteAsync([FromRoute] Guid Id, [FromBody] UpdateTodoIsCompletedRequest request)
        {
            var (todo, lastEventNumber) = await _service.UpdateTodoIsCompletedAsync(request.ExpectedVersion, Id,
                request.IsCompleted);

            if (todo == null) return NotFound();

            return Ok(new GeneralPostTodoResponse(todo.Id, lastEventNumber));
        }
    }
}