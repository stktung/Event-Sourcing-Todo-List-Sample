namespace SleekFlow.Todo.Application.Model;

/// <summary>
/// 
/// </summary>
/// <param name="ExpectedVersion">The version expected when Todo is saved. If not matched, a version mismatch error will be returned</param>
/// <param name="Text">Text to insert</param>
/// <param name="Position">Zero based position of where the text is inserted</param>
public record InsertTodoNameTextRequest(long ExpectedVersion, string Text, int Position);