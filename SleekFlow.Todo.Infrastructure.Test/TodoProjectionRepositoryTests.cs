using SleekFlow.Todo.Domain.Aggregate;

namespace SleekFlow.Todo.Infrastructure.Test;

public class TodoProjectionRepositoryTests
{
    [Test]
    public async Task TodoProjectionRepository_Simple_Save_And_GetAll_Returns_Todos()
    {
        var store = new EmbeddedEventStoreDb.EmbeddedEventStoreDb();
        var db = new EmbeddedSqliteDB.EmbeddedSqliteDb();
        var todoRepo = new TodoRepository(store);
        var projectionRepo = new TodoProjectionRepository(store, db);
        var todo1 = TodoAggregate.Create();
        var todo2 = TodoAggregate.Create();

        await todoRepo.SaveAsync(todo1);
        await todoRepo.SaveAsync(todo2);

        await projectionRepo.Save(todo1.Id);
        await projectionRepo.Save(todo2.Id);

        var todoProjections = await projectionRepo.GetAllAsync();

        Assert.That(todoProjections.ToList()[0].Id, Is.EqualTo(todo1.Id));
        Assert.That(todoProjections.ToList()[1].Id, Is.EqualTo(todo2.Id));
    }

}