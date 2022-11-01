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

        public async Task<Guid> CreateTodoAsync()
        {
            var todo = TodoItem.Create();

            await _repository.Save(todo);

            return todo.Id;
        }

        public async Task<TodoItem?> GetAsync(Guid id)
        {
            return await _repository.GetAsync(id);
        }
    }
}