namespace SleekFlow.Todo.Application.Model;

public record GetTodoResponse(Guid Id, string? Name, string? Description, DateTime? DueDate, bool Completed,
    DateTime LastUpdatedAt, long LastEventNumber);