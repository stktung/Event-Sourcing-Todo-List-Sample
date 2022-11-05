using Dapper;
using Microsoft.Data.Sqlite;

namespace SleekFlow.Todo.Infrastructure.EmbeddedSqliteDB
{
    public class EmbeddedSqliteDb : ISqlDb, IDisposable
    {
        private const string CreateTableTodoProjection =
            @"CREATE TABLE TodoProjections (
    Id TEXT PRIMARY KEY,
    Name TEXT NULL,
    Description TEXT NULL,
    DueDate DATETIME2 NULL,
    Completed BIT NOT NULL,
    LastUpdatedAt DATETIME2 NOT NULL,
    LastEventNumber BIGINT NOT NULL
)";

        public SqliteConnection Connection { get; }

        public EmbeddedSqliteDb()
        {
            Connection = new SqliteConnection("Data Source=:memory:");
            Connection.Open();

            CreateDefaultTables();

        }

        private void CreateDefaultTables()
        {
            Connection.Execute(CreateTableTodoProjection);
        }

        public void Dispose()
        {
            Connection.Close();
            Connection.Dispose();
        }
    }
}