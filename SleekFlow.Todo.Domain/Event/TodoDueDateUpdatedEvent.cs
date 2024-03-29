﻿namespace SleekFlow.Todo.Domain.Event;

public class TodoDueDateUpdatedEvent : Common.DomainEvent
{
    public DateTime? DueDate { get; set; }

    public TodoDueDateUpdatedEvent() : base(nameof(TodoDueDateUpdatedEvent))
    {
    }
}