namespace SleekFlow.Todo.Domain.Aggregate
{
    public class TodoItemAggregate : EventSourcedAggregate
    {
        public Guid Id { get; private set; }

        private TodoItemAggregate()
        {
        }

        public static TodoItemAggregate Create()
        {
            var todo = new TodoItemAggregate();
            todo.Id = Guid.NewGuid();

            todo.Raise(new TodoCreatedEvent());
            
            return todo;
        }

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

        public static TodoItemAggregate Load(IEnumerable<IEvent> events)
        {
            var todo = new TodoItemAggregate();
            
            foreach (var e in events)
            {
                todo.Apply(e);
            }

            return todo;
        }
    }
}
