using SleekFlow.Todo.Domain.Aggregate;

namespace SleekFlow.Todo.Domain
{
    public class TodoService : ITodoService
    {
        private readonly ITodoRepository _repository;

        public TodoService(ITodoRepository repository)
        {
            _repository = repository;
        }

        public async Task<(Guid Id, long LastEventNumber)> CreateTodoAsync()
        {
            var todo = TodoAggregate.Create();

            var latestEventNumber = await _repository.SaveAsync(todo);

            return (todo.Id, latestEventNumber);
        }

        public async Task<(TodoAggregate? Todo, long LastEventNumber)> InsertTodoNameTextAsync(long expectedVersion,
            Guid id, string text, int position)
        {
            var todo = await _repository.LoadLatestAsync(id, expectedVersion);

            if (todo == null) return (null, -1);

            todo.InsertTextToName(text, position);

            var lastEventNumber = await _repository.SaveAsync(todo);

            return (todo, lastEventNumber);
        }
    }
}