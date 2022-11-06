using SleekFlow.Todo.Domain.Aggregate;
using SleekFlow.Todo.Domain.Common;

namespace SleekFlow.Todo.Domain
{
    public class TodoService : ITodoService
    {
        private readonly ITodoRepository _repository;

        public TodoService(ITodoRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<DomainEvent>?> GetHistory(Guid id)
        {
            return await _repository.GetEventsAsync(id);
        }

        public async Task<(Guid Id, long LastEventNumber)> CreateTodoAsync()
        {
            var todo = TodoAggregate.Create();

            var latestEventNumber = await _repository.SaveAsync(todo);

            return (todo.Id, latestEventNumber);
        }

        public async Task<(TodoAggregate todo, long lastEventNumber)> InsertTodoNameTextAsync(long expectedVersion,
            Guid id, string text, int position)
        {
            var todo = await _repository.LoadLatestAsync(id, expectedVersion);

            if (todo == null) throw new KeyNotFoundException($"Todo not found. Id: '{id}'");

            todo.InsertTextToName(text, position);

            var lastEventNumber = await _repository.SaveAsync(todo);

            return (todo, lastEventNumber);
        }

        public async Task<(TodoAggregate todo, long lastEventNumber)> DeleteTodoNameTextAsync(long expectedVersion,
            Guid id, int position, int length)
        {
            var todo = await _repository.LoadLatestAsync(id, expectedVersion);

            if (todo == null) throw new KeyNotFoundException($"Todo not found. Id: '{id}'");

            todo.DeleteTextFromName(position, length);

            var lastEventNumber = await _repository.SaveAsync(todo);

            return (todo, lastEventNumber);
        }

        public async Task<(TodoAggregate todo, long lastEventNumber)> InsertTodoDescriptionTextAsync(long expectedVersion,
            Guid id, string text, int position)
        {
            var todo = await _repository.LoadLatestAsync(id, expectedVersion);

            if (todo == null) throw new KeyNotFoundException($"Todo not found. Id: '{id}'");

            todo.InsertTextToDescription(text, position);

            var lastEventNumber = await _repository.SaveAsync(todo);

            return (todo, lastEventNumber);
        }

        public async Task<(TodoAggregate todo, long lastEventNumber)> DeleteTodoDescriptionTextAsync(long expectedVersion,
            Guid id, int position, int length)
        {
            var todo = await _repository.LoadLatestAsync(id, expectedVersion);

            if (todo == null) throw new KeyNotFoundException($"Todo not found. Id: '{id}'");

            todo.DeleteTextFromDescription(position, length);

            var lastEventNumber = await _repository.SaveAsync(todo);

            return (todo, lastEventNumber);
        }

        public async Task<(TodoAggregate todo, long lastEventNumber)> UpdateTodoDueDateAsync(long expectedVersion, Guid id, DateTime? dueDate)
        {
            var todo = await _repository.LoadLatestAsync(id, expectedVersion);

            if (todo == null) throw new KeyNotFoundException($"Todo not found. Id: '{id}'");

            var dueDateInUtc = DateTimeHelper.ConvertToUtc(dueDate);

            todo.UpdateDueDate(dueDateInUtc);

            var lastEventNumber = await _repository.SaveAsync(todo);

            return (todo, lastEventNumber);
        }

        public async Task<(TodoAggregate todo, long lastEventNumber)> UpdateTodoIsCompletedAsync(long expectedVersion, Guid id, bool isCompleted)
        {
            var todo = await _repository.LoadLatestAsync(id, expectedVersion);

            if (todo == null) throw new KeyNotFoundException($"Todo not found. Id: '{id}'");
            
            todo.UpdateIsCompleted(isCompleted);

            var lastEventNumber = await _repository.SaveAsync(todo);

            return (todo, lastEventNumber);
        }
    }
}