using System.Data.SQLite;

namespace SleekFlow.Todo.Infrastructure.EmbeddedSqliteDB;

public interface ISqlDb
{
    SQLiteConnection Connection { get; }
}