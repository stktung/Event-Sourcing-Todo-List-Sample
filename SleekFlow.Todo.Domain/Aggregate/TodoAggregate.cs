using SleekFlow.Todo.Domain.Common;
using SleekFlow.Todo.Domain.Event;

namespace SleekFlow.Todo.Domain.Aggregate
{
    public class TodoAggregate : EventSourcedAggregate
    {
        private bool _created = false;
        private long _nameLength = 0;

        private TodoAggregate() : base("Todo")
        {
        }

        public static TodoAggregate Create()
        {
            var todo = new TodoAggregate
            {
                Id = Guid.NewGuid()
            };

            todo._created = true;

            todo.Raise(new TodoCreatedEvent { Id = todo.Id });
            
            return todo;
        }

        public void InsertTextToName(string text, int position)
        {
            if (!_created) throw new DomainException("Todo has not been created.");

            if (position >= _nameLength)
                throw new DomainException($"Can not insert beyond end of text. TextLength = '{_nameLength}'");
            
            if (position < 0)
                throw new DomainException($"Position must be greater than 0. Position = '{position}'");

            Raise(new TodoNameTextInsertedEvent { Id = Id, Text = text, Position = position });
        }

        private void Apply(Common.DomainEvent e) 
        {
            switch (e)
            {
                case TodoCreatedEvent evt:
                    Apply(evt);
                    break;
                case TodoNameTextInsertedEvent evt:
                    Apply(evt);
                    break;

            }
        }

        private void Apply(TodoCreatedEvent e)
        {
            Id = e.Id;
            _created = true;
        }
        private void Apply(TodoNameTextInsertedEvent e)
        {
            _nameLength += e.Text.Length;
        }

        public static TodoAggregate Load(IEnumerable<Common.DomainEvent> events)
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
