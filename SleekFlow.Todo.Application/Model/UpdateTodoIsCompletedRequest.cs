namespace SleekFlow.Todo.Application.Model;

/// <summary>
/// 
/// </summary>
/// <param name="ExpectedVersion">The version expected when Todo is saved. If not matched, a version mismatch error will be returned</param>
/// <param name="IsCompleted"></param>
public record UpdateTodoIsCompletedRequest(long ExpectedVersion, bool IsCompleted);