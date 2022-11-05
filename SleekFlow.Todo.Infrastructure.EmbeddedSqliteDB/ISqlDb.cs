using Microsoft.Data.Sqlite;

namespace SleekFlow.Todo.Infrastructure.EmbeddedSqliteDB;

public interface ISqlDb
{
    SqliteConnection Connection { get; }
}