namespace SleekFlow.Todo.Domain.Projection
{
    public class TodoItemProjection
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public bool Completed { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public long LastEventNumber { get; set; }

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
            Name = null;
            Description = null;
            DueDate = null;
            Completed = false;
            LastUpdatedAt = e.RaisedAt;
            LastEventNumber = e.EventNumber;
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
