using SleekFlow.Todo.Domain.Aggregate;

namespace SleekFlow.Todo.Domain.Projection
{
    public class TodoItemProjection
    {
        public Guid Id { get; set; }

        private void Apply(IEvent e)
        {
            switch (e)
            {
                case TodoCreatedEvent todoCreatedEvent:
                    Apply(todoCreatedEvent);
                    break;
            }
        }

        private void Apply(TodoCreatedEvent e)
        {
            Id = e.Id;
        }

        public static TodoItemProjection Load(IEnumerable<IEvent> events)
        {
            var todo = new TodoItemProjection();

            foreach (var e in events)
            {
                todo.Apply(e);
            }

            return todo;
        }
    }
}
