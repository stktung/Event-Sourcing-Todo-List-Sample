﻿using SleekFlow.Todo.Domain.Common;
using SleekFlow.Todo.Domain.Event;
using static System.Net.Mime.MediaTypeNames;

namespace SleekFlow.Todo.Domain.Aggregate
{
    public class TodoAggregate : EventSourcedAggregate
    {
        private bool _created;
        private bool _completed;
        private long _nameLength;
        private long _descriptionLength;

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
            if (!_created) throw new DomainException("Todo has not been created yet");
            if (_completed) throw new DomainException("Todo is completed and can not be updated");

            if (position < 0)
                throw new DomainException($"Position must be greater than 0. Position = '{position}'");

            if (position > _nameLength)
                throw new DomainException($"Can not insert to a point beyond end of text. TextLength: '{_nameLength}' Position: '{position}'");
            
            Raise(new TodoNameTextInsertedEvent { Id = Id, Text = text, Position = position });
        }

        public void DeleteTextFromName(int position, int length)
        {
            if (!_created) throw new DomainException("Todo has not been created yet");
            if (_completed) throw new DomainException("Todo is completed and can not be updated");

            if (position < 0)
                throw new DomainException($"Position must be greater than 0. Position = '{position}'");

            if (position > _nameLength)
                throw new DomainException($"Can not delete from a position beyond end of text. TextLength: '{_nameLength}' Position: '{position}'");

            if (position + length > _nameLength)
                throw new DomainException($"Can not delete text from beyond end of text. TextLength: '{_nameLength}' Position: '{position}' LengthToDelete: '{length}'");

            Raise(new TodoNameTextDeletedEvent { Id = Id, Position = position, Length = length });
        }

        public void InsertTextToDescription(string text, int position)
        {
            if (!_created) throw new DomainException("Todo has not been created yet");
            if (_completed) throw new DomainException("Todo is completed and can not be updated");

            if (position < 0)
                throw new DomainException($"Position must be greater than 0. Position = '{position}'");

            if (position > _descriptionLength)
                throw new DomainException($"Can not insert to a point beyond end of text. TextLength: '{_descriptionLength}' Position: '{position}'");

            Raise(new TodoDescriptionTextInsertedEvent { Id = Id, Text = text, Position = position });
        }

        public void DeleteTextFromDescription(int position, int length)
        {
            if (!_created) throw new DomainException("Todo has not been created yet");
            if (_completed) throw new DomainException("Todo is completed and can not be updated");

            if (position < 0)
                throw new DomainException($"Position must be greater than 0. Position = '{position}'");

            if (position > _descriptionLength)
                throw new DomainException($"Can not delete from a position beyond end of text. TextLength: '{_descriptionLength}' Position: '{position}'");

            if (position + length > _descriptionLength)
                throw new DomainException($"Can not delete text from beyond end of text. TextLength: '{_descriptionLength}' Position: '{position}' LengthToDelete: '{length}'");

            Raise(new TodoDescriptionTextDeletedEvent { Id = Id, Position = position, Length = length });
        }

        public void UpdateDueDate(DateTime? dueDate)
        {
            if (!_created) throw new DomainException("Todo has not been created yet");
            if (_completed) throw new DomainException("Todo is completed and can not be updated");

            if (dueDate == null)
            {
                Raise(new TodoDueDateUpdatedEvent { Id = Id, DueDate = null });
            }
            else
            {
                if (dueDate.Value.Kind != DateTimeKind.Utc)
                    throw new DomainException($"Due date must be in UTC format");

                Raise(new TodoDueDateUpdatedEvent { Id = Id, DueDate = dueDate });
            }
        }
        public void UpdateIsCompleted(bool isCompleted)
        {
            if (!_created) throw new DomainException("Todo has not been created yet");

            _completed = isCompleted;

            if (isCompleted) 
                Raise(new TodoCompleteMarkedEvent { Id = Id, });
            else
                Raise(new TodoCompleteUnmarkedEvent { Id = Id, });
        }

        private void Apply(DomainEvent e) 
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

            _pastEvents.Add(e);
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

        private void Apply(TodoNameTextDeletedEvent e)
        {
            _nameLength -= e.Length;
        }

        private void Apply(TodoDescriptionTextInsertedEvent e)
        {
            _descriptionLength += e.Text.Length;
        }

        private void Apply(TodoDescriptionTextDeletedEvent e)
        {
            _descriptionLength -= e.Length;
        }
        private void Apply(TodoDueDateUpdatedEvent e)
        {
            // no implementation required
        }
        private void Apply(TodoCompleteMarkedEvent e)
        {
            _completed = true;
        }
        private void Apply(TodoCompleteUnmarkedEvent e)
        {
            _completed = false;
        }

        public static TodoAggregate Load(IEnumerable<DomainEvent> events)
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
