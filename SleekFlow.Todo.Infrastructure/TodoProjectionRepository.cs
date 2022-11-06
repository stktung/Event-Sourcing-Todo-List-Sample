using Dapper;
using SleekFlow.Todo.Domain;
using SleekFlow.Todo.Domain.Projection;
using SleekFlow.Todo.Infrastructure.EmbeddedEventStoreDb;
using SleekFlow.Todo.Infrastructure.EmbeddedSqliteDB;

namespace SleekFlow.Todo.Infrastructure;

public class TodoProjectionRepository : ITodoProjectionRepository
{
    private const string InsertSql =
        @"INSERT INTO TodoProjections 
VALUES (@Id, @Name, @Description, @DueDate, @Completed, @LastUpdatedAt, @LastEventNumber)";

    private const string UpdateSql =
        @"UPDATE TodoProjections 
SET Name = @Name, Description = @Description, DueDate = @DueDate, Completed = @Completed, LastUpdatedAt = @LastUpdatedAt, LastEventNumber = @LastEventNumber 
WHERE Id = @Id";

    private readonly IEventStore _eventStore;
    private readonly ISqlDb _db;

    public TodoProjectionRepository(IEventStore eventStore, ISqlDb db)
    {
        _eventStore = eventStore;
        _db = db;
    }

    public async Task<TodoProjection?> GetFromEventStoreAsync(Guid id)
    {
        var events = await _eventStore.ReadAllAsync(TodoRepository.BuildStreamName(id));

        if (events == null) return null;

        var domainEvents = events.Select(EventMapper.MapToDomainEvent).ToList();

        return TodoProjection.Load(domainEvents);
    }

    public async Task<IEnumerable<TodoProjection>?> GetAllAsync()
    {
        var todos = await _db.Connection.QueryAsync($"SELECT * FROM TodoProjections");

        if (todos == null || !todos.Any()) return null;

        var list = new List<TodoProjection>();
        foreach (var todo in todos)
        {
            list.Add(new TodoProjection()
            {
                Id = Guid.Parse(todo.Id), 
                Name = todo.Name, 
                Description = todo.Description,
                DueDate = ConvertToUtcDateTime(todo.DueDate),
                Completed = Convert.ToBoolean(todo.Completed), 
                LastUpdatedAt = ConvertToUtcDateTime(todo.LastUpdatedAt),
                LastEventNumber = todo.LastEventNumber
            });
        }

        return list;
    }

    public async Task Save(Guid id)
    {
        var todo = await GetFromEventStoreAsync(id);

        if (todo == null) throw new ArgumentException($"Id '{id}' not found");

        var count = await _db.Connection.ExecuteScalarAsync<int>($"SELECT COUNT(1) FROM TodoProjections WHERE Id = '{id}'");

        if (count == 0)
        {
            await _db.Connection.ExecuteAsync(InsertSql,
                new
                {
                    Id = todo.Id.ToString(),
                    todo.Name,
                    todo.Description,
                    todo.DueDate,
                    todo.Completed,
                    todo.LastUpdatedAt,
                    todo.LastEventNumber
                });
        }
        else
        {
            await _db.Connection.ExecuteAsync(UpdateSql,
                new
                {
                    Id = todo.Id.ToString(),
                    todo.Name,
                    todo.Description,
                    todo.DueDate,
                    todo.Completed,
                    todo.LastUpdatedAt,
                    todo.LastEventNumber
                });
        }

    }

    private static DateTime? ConvertToUtcDateTime(string? s)
    {
        if (s == null) return null;
        
        return Convert.ToDateTime(s).ToUniversalTime();
    }
}