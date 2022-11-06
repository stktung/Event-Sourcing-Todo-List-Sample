using SleekFlow.Todo.Domain.Common;
using SleekFlow.Todo.Domain.Event;

namespace SleekFlow.Todo.Domain.Projection
{
    public class TodoProjection
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public bool Completed { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public long LastEventNumber { get; set; }

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
                case TodoNameTextDeletedEvent evt:
                    Apply(evt);
                    break;
                case TodoDescriptionTextInsertedEvent evt:
                    Apply(evt);
                    break;
                case TodoDescriptionTextDeletedEvent evt:
                    Apply(evt);
                    break;
                case TodoDueDateUpdatedEvent evt:
                    Apply(evt);
                    break;
                case TodoCompleteMarkedEvent evt:
                    Apply(evt);
                    break;
                case TodoCompleteUnmarkedEvent evt:
                    Apply(evt);
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

        private void Apply(TodoNameTextInsertedEvent e)
        {
            if (e.Position < 0)
                throw new ProjectionException($"Position must be greater than or equals to 0. Position:'{e.Position}'");
            if (string.IsNullOrEmpty(Name) && e.Position > 0)
                throw new ProjectionException(
                    $"Position not exceed length of name. Position: '{e.Position}' Name length: '0'");
            if (!string.IsNullOrEmpty(Name) && e.Position > Name.Length)
                throw new ProjectionException(
                    $"Position not exceed length of name. Position: '{e.Position}' Name length: '{Name.Length}'");

            Name = string.IsNullOrEmpty(Name) ? e.Text : Name.Insert(e.Position, e.Text);

            LastUpdatedAt = e.RaisedAt;
            LastEventNumber = e.EventNumber;
        }
        private void Apply(TodoNameTextDeletedEvent e)
        {
            if (e.Position < 0)
                throw new ProjectionException($"Position must be greater than or equals to 0. Position:'{e.Position}'");
            if (string.IsNullOrEmpty(Name) && e.Position > 0)
                throw new ProjectionException(
                    $"Position not exceed length of name. Position: '{e.Position}' Name length: '0'");
            if (string.IsNullOrEmpty(Name) && e.Position + e.Length > 0)
                throw new ProjectionException(
                    $"Can not delete text beyond the text. Position: '{e.Position}' Name length: '0' Length to Delete: '{e.Length}'");
            if (!string.IsNullOrEmpty(Name) && e.Position > Name.Length)
                throw new ProjectionException(
                    $"Position not exceed length of name. Position: '{e.Position}' Name length: '{Name.Length}'");
            if (!string.IsNullOrEmpty(Name) && e.Position + e.Length > Name.Length)
                throw new ProjectionException(
                    $"Can not delete text beyond the text. Position: '{e.Position}' Name length: '{Name.Length}' Length to Delete: '{e.Length}'");

            if (Name == null) return;
            
            Name = Name.Remove(e.Position, e.Length);
            LastUpdatedAt = e.RaisedAt;
            LastEventNumber = e.EventNumber;
        }
        private void Apply(TodoDescriptionTextInsertedEvent e)
        {
            if (e.Position < 0)
                throw new ProjectionException($"Position must be greater than or equals to 0. Position:'{e.Position}'");
            if (string.IsNullOrEmpty(Description) && e.Position > 0)
                throw new ProjectionException(
                    $"Position not exceed length of description. Position: '{e.Position}' description length: '0'");
            if (!string.IsNullOrEmpty(Description) && e.Position > Description.Length)
                throw new ProjectionException(
                    $"Position not exceed length of description. Position: '{e.Position}' description length: '{Description.Length}'");

            Description = string.IsNullOrEmpty(Description) ? e.Text : Description.Insert(e.Position, e.Text);

            LastUpdatedAt = e.RaisedAt;
            LastEventNumber = e.EventNumber;
        }

        private void Apply(TodoDescriptionTextDeletedEvent e)
        {
            if (e.Position < 0)
                throw new ProjectionException($"Position must be greater than or equals to 0. Position:'{e.Position}'");
            if (string.IsNullOrEmpty(Description) && e.Position > 0)
                throw new ProjectionException(
                    $"Position not exceed length of description. Position: '{e.Position}' description length: '0'");
            if (string.IsNullOrEmpty(Description) && e.Position + e.Length > 0)
                throw new ProjectionException(
                    $"Can not delete text beyond the text. Position: '{e.Position}' description length: '0' Length to Delete: '{e.Length}'");
            if (!string.IsNullOrEmpty(Description) && e.Position > Description.Length)
                throw new ProjectionException(
                    $"Position not exceed length of description. Position: '{e.Position}' description length: '{Description.Length}'");
            if (!string.IsNullOrEmpty(Description) && e.Position + e.Length > Description.Length)
                throw new ProjectionException(
                    $"Can not delete text beyond the text. Position: '{e.Position}' description length: '{Description.Length}' Length to Delete: '{e.Length}'");

            if (Description == null) return;
            
            Description = Description.Remove(e.Position, e.Length);
            LastUpdatedAt = e.RaisedAt;
            LastEventNumber = e.EventNumber;
        }

        private void Apply(TodoDueDateUpdatedEvent e)
        {
            if (e.DueDate != null && e.DueDate.Value.Kind != DateTimeKind.Utc)
                throw new ProjectionException($"Due date must be in UTC format");

            DueDate = e.DueDate;

            LastUpdatedAt = e.RaisedAt;
            LastEventNumber = e.EventNumber;
        }

        private void Apply(TodoCompleteMarkedEvent e)
        {
            Completed = true;

            LastUpdatedAt = e.RaisedAt;
            LastEventNumber = e.EventNumber;
        }

        private void Apply(TodoCompleteUnmarkedEvent e)
        {
            Completed = false;

            LastUpdatedAt = e.RaisedAt;
            LastEventNumber = e.EventNumber;
        }

        public static TodoProjection Load(IEnumerable<Common.DomainEvent> events)
        {
            var todo = new TodoProjection();

            foreach (var e in events)
            {
                todo.Apply(e);
            }

            return todo;
        }
    }
}
