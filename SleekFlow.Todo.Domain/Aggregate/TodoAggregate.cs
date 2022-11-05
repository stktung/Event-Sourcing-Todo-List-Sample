using SleekFlow.Todo.Domain.Common;

namespace SleekFlow.Todo.Domain.Aggregate
{
    public class TodoAggregate : EventSourcedAggregate
    {
        private TodoAggregate() : base("Todo")
        {
        }

        public static TodoAggregate Create()
        {
            var todo = new TodoAggregate
            {
                Id = Guid.NewGuid()
            };

            todo.Raise(new TodoCreatedEvent { Id = todo.Id });
            
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

        public static TodoAggregate Load(IEnumerable<IEvent> events)
        {
            var todo = new TodoAggregate();
            
            foreach (var e in events)
            {
                todo.Apply(e);
            }

            return todo;
        }
    }
}
