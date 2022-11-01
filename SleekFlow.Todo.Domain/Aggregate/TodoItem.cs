using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleekFlow.Todo.Domain.Aggregate
{
    public class TodoItem : EventSourcedAggregate
    {
        public Guid Id { get; } = new();

        private TodoItem()
        {
        }

        public static TodoItem Create()
        {
            var todo = new TodoItem();
            todo.Raise(new TodoCreatedEvent());
            
            return todo;
        }
    }

    public abstract class EventSourcedAggregate
    {
        private readonly List<IEvent> _pastEvents = new();
        private readonly List<IEvent> _newEvents = new();

        protected void Raise(IEvent e)
        {
            _newEvents.Add(e);
        }

        public int PreviousRevision => _pastEvents.Count() - 1;

        public ReadOnlyCollection<IEvent> NewEvents => _newEvents.AsReadOnly();

    }
}
