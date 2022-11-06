using SleekFlow.Todo.Domain.Aggregate;

namespace SleekFlow.Todo.Infrastructure.Test;

public class TodoRepositoryTests
{
    [Test]
    public async Task TodoRepository_Simple_Save_And_Get_Returns_Todo()
    {
        var store = new EmbeddedEventStoreDb.EmbeddedEventStoreDb();
        var db = new EmbeddedSqliteDB.EmbeddedSqliteDb();
        var todoRepo = new TodoRepository(store);
        var projectionRepo = new TodoProjectionRepository(store, db);
        var todo = TodoAggregate.Create();

        await todoRepo.SaveAsync(todo);
        var todoProjection = await projectionRepo.GetFromEventStoreAsync(todo.Id);

        Assert.That(todoProjection.Id, Is.EqualTo(todo.Id));
    }

}