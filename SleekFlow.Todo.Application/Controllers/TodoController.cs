using AutoMapper;
using EventStore.Transport.Http;
using Microsoft.AspNetCore.Mvc;
using SleekFlow.Todo.Application.Middleware;
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

        /// <summary>
        /// Get properties of a Todo
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Todo object with its properties</returns>
        [Route("{id:guid}")]
        [ProducesResponseType(typeof(GetTodoResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(404)]
        [Produces(ContentType.Json)]
        [HttpGet]
        public async Task<IActionResult> GetAsync([FromRoute] Guid id)
        {
            var todo = await _projectionRepository.GetFromEventStoreAsync(id);
            if (todo == null) return NotFound();

            return Ok(_mapper.Map<GetTodoResponse>(todo));
        }

        /// <summary>
        /// Get all the events of a Todo
        /// </summary>
        /// <param name="id"></param>
        /// <returns>List of events of the Todo</returns>
        [Route("{id:guid}/history")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DomainEventWebDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(404)]
        [Produces(ContentType.Json)]
        public async Task<IActionResult> GetHistoryAsync([FromRoute] Guid id)
        {
            var events = await _service.GetHistory(id);
            if (events == null) return NotFound();

            return Ok(events.Select(e => _mapper.Map(e, e.GetType(), typeof(DomainEventWebDto))));
        }

        /// <summary>
        /// Get all Todos and optionally filter and/or sort by certain fields
        /// </summary>
        /// <param name="isCompleted">Filter by todos that completed</param>
        /// <param name="dueDateIsBefore">Filter by todos with due date before this. E.g. 2022-11-30</param>
        /// <param name="dueDateIsAfter">Filter by todos with due date after this. E.g. 2022-11-01 </param>
        /// <param name="sortByField">Sort by this field. Currently only "Name" (0) and "DueDate" (1) is supported</param>
        /// <param name="sortByAsc">Sort asc if this is true, otherwise sorts descending</param>
        /// <returns>List of Todos</returns>
        /// <response code="200">Returns if one or more Todo is found</response>
        /// <response code="404">Returns if no Todo can found</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<GetTodoResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(404)]
        [Produces(ContentType.Json)]
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

        /// <summary>
        /// Creates a new empty Todo
        /// </summary>
        /// <returns>Todo Id and last event number </returns>
        [ProducesResponseType(typeof(GetTodoResponse), (int)HttpStatusCode.OK)]
        [Produces(ContentType.Json)]
        [HttpPost("create")]
        public async Task<GeneralPostTodoResponse> CreateTodoAsync()
        {
            var result = await _service.CreateTodoAsync();

            return new GeneralPostTodoResponse(result.Id, result.LastEventNumber);
        }

        /// <summary>
        /// Inserts piece of text to Todo's name at specified position
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("{Id:guid}/name/inserttext")]
        [ProducesResponseType(typeof(GetTodoResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), HttpStatusCode.NotFound)]
        [Produces(ContentType.Json)]
        [HttpPost]
        public async Task<IActionResult> InsertTodoNameTextAsync([FromRoute] Guid Id, [FromBody] InsertTodoNameTextRequest request)
        {
            var (todo, lastEventNumber) = await _service.InsertTodoNameTextAsync(request.ExpectedVersion, Id,
                request.Text, request.Position);

            if (todo == null) return NotFound();

            return Ok(new GeneralPostTodoResponse(todo.Id, lastEventNumber));
        }

        /// <summary>
        /// Deletes piece of text from Todo's name at specified position
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(GetTodoResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), HttpStatusCode.NotFound)]
        [Produces(ContentType.Json)]
        [Route("{Id:guid}/name/deletetext")]
        [HttpPost]
        public async Task<IActionResult> DeleteTodoNameTextAsync([FromRoute] Guid Id, [FromBody] DeleteTodoNameTextRequest request)
        {
            var (todo, lastEventNumber) = await _service.DeleteTodoNameTextAsync(request.ExpectedVersion, Id,
                request.Position, request.length);

            if (todo == null) return NotFound();

            return Ok(new GeneralPostTodoResponse(todo.Id, lastEventNumber));
        }

        /// <summary>
        /// Inserts piece of text to Todo's description at specified position
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(GetTodoResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [Produces(ContentType.Json)]
        [Route("{Id:guid}/description/inserttext")]
        [HttpPost]
        public async Task<IActionResult> InsertTodoDescriptionTextAsync([FromRoute] Guid Id, [FromBody] InsertTodoDescriptionTextRequest request)
        {
            var (todo, lastEventNumber) = await _service.InsertTodoDescriptionTextAsync(request.ExpectedVersion, Id,
                request.Text, request.Position);

            if (todo == null) return NotFound();

            return Ok(new GeneralPostTodoResponse(todo.Id, lastEventNumber));
        }

        /// <summary>
        /// Deletes piece of text from Todo's description at specified position
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(GetTodoResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [Produces(ContentType.Json)]
        [Route("{Id:guid}/description/deletetext")]
        [HttpPost]
        public async Task<IActionResult> DeleteTodoDescriptionTextAsync([FromRoute] Guid Id, [FromBody] DeleteTodoDescriptionTextRequest request)
        {
            var (todo, lastEventNumber) = await _service.DeleteTodoDescriptionTextAsync(request.ExpectedVersion, Id,
                request.Position, request.length);

            if (todo == null) return NotFound();

            return Ok(new GeneralPostTodoResponse(todo.Id, lastEventNumber));
        }

        /// <summary>
        /// Updates Todo's due date
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(GetTodoResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [Produces(ContentType.Json)]
        [Route("{Id:guid}/duedate")]
        [HttpPut]
        public async Task<IActionResult> UpdateTodoDueDateAsync([FromRoute] Guid Id, [FromBody] UpdateTodoDueDateRequest request)
        {
            var (todo, lastEventNumber) = await _service.UpdateTodoDueDateAsync(request.ExpectedVersion, Id,
                request.DueDate);

            if (todo == null) return NotFound();

            return Ok(new GeneralPostTodoResponse(todo.Id, lastEventNumber));
        }

        /// <summary>
        /// Set or unset a Todo's completion status
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(GetTodoResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [Produces(ContentType.Json)]
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