using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleekFlow.Todo.Domain.Aggregate
{
    public class TodoItem : EventSourcedAggregate
    {
        public Guid Id { get; private set; } = new();

        private TodoItem()
        {
        }

        public static TodoItem Create()
        {
            var todo = new TodoItem();
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

        public static TodoItem Load(IEnumerable<IEvent> events)
        {
            var todo = new TodoItem();
            
            foreach (var e in events)
            {
                todo.Apply(e);
            }

            return todo;
        }
    }
}
