namespace SleekFlow.Todo.Application.Model;

/// <summary>
/// 
/// </summary>
/// <param name="ExpectedVersion">The version expected when Todo is saved. If not matched, a version mismatch error will be returned</param>
/// <param name="Position">Zero based position of where deletion begins</param>
/// <param name="length">Number characters to delete from position</param>
public record DeleteTodoDescriptionTextRequest(long ExpectedVersion, int Position, int length);