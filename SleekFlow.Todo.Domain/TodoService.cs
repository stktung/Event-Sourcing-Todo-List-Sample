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

        public async Task<Guid> CreateTodoAsync()
        {
            var todo = TodoItemAggregate.Create();

            await _repository.Save(todo);

            return todo.Id;
        }

        public async Task<TodoItemProjection?> GetAsync(Guid id)
        {
            return await _repository.GetAsync(id);
        }
    }
}