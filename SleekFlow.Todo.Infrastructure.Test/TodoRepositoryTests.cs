using SleekFlow.Todo.Domain.Aggregate;
using SleekFlow.Todo.Infrastructure.EmbeddedEventStoreDB;

namespace SleekFlow.Todo.Infrastructure.Test;

public class TodoRepositoryTests
{
    [Test]
    public async Task TodoRepository_Simple_Save_And_Get_Returns_Todo()
    {
        var db = new EmbeddedEventStoreDb();
        var repo = new TodoRepository(db);
        var todo = TodoItemAggregate.Create();

        await repo.Save(todo);
        var todoItemProjection = await repo.GetAsync(todo.Id);

        Assert.That(todoItemProjection.Id, Is.EqualTo(todo.Id));
    }

}