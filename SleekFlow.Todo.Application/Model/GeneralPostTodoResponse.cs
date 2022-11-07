namespace SleekFlow.Todo.Application.Model;

/// <summary>
/// 
/// </summary>
/// <param name="Id">Updated Todo's Id</param>
/// <param name="LastEventNumber">The latest event number of the updated Todo</param>
public record GeneralPostTodoResponse(Guid Id, long LastEventNumber);