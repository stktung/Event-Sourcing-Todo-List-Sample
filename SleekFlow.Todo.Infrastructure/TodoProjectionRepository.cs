using Dapper;
using SleekFlow.Todo.Domain;
using SleekFlow.Todo.Domain.Projection;
using SleekFlow.Todo.Infrastructure.EmbeddedEventStoreDb;
using SleekFlow.Todo.Infrastructure.EmbeddedSqliteDB;

namespace SleekFlow.Todo.Infrastructure;

public class TodoProjectionRepository : ITodoProjectionRepository
{
    private const string InsertTableTodoProjection =
        @"INSERT INTO TodoProjections VALUES (@Id, @Name, @Description, @DueDate, @Completed, @LastUpdatedAt, @LastEventNumber)";

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

        var domainEvents = events.Select(esEvent => EventMapper.MapToDomainEvent(esEvent)).ToList();

        return TodoProjection.Load(domainEvents);
    }

    public async Task Save(Guid id)
    {
        var todo = await GetFromEventStoreAsync(id);

        if (todo == null) throw new ArgumentException($"Id '{id}' not found");

        await _db.Connection.ExecuteAsync(InsertTableTodoProjection,
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

    private static DateTime? ConvertToUtcDateTime(string? s)
    {
        if (s == null) return null;
        
        return DateTime.SpecifyKind(Convert.ToDateTime(s), DateTimeKind.Utc);
    }
}