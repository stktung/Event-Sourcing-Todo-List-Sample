using SleekFlow.Todo.Domain.Aggregate;
using SleekFlow.Todo.Domain.Projection;

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

            var latestEventNumber = await _repository.Save(todo);

            return (todo.Id, latestEventNumber);
        }
    }
}