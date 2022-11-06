namespace SleekFlow.Todo.IntegrationTest;

public class UpdateTodoDueDateIntegrationTest : TodoIntegrationTest
{
    [Test]
    public async Task TodoController_UpdateTodoDueDate_Simple_Returns_Todo_With_DueDate()
    {
        var createResp = await _client.CreateTodoAsync();
        var dueDate = DateTime.UtcNow;

        var before = DateTime.UtcNow;
        var resp = (await _client.UpdateTodoDueDateAsync(createResp.Id, createResp.LastEventNumber, dueDate)).Response;
        var after = DateTime.UtcNow;

        await Task.Delay(TimeSpan.FromSeconds(1));

        var todo = (await _client.GetAllAsync()).First();

        Assert.That(resp.Id, Is.EqualTo(createResp.Id));
        Assert.That(resp.LastEventNumber, Is.EqualTo(1));

        Assert.That(todo.Id, Is.EqualTo(createResp.Id));
        Assert.That(todo.Name, Is.Null);
        Assert.That(todo.Description, Is.Null);
        Assert.That(todo.DueDate, Is.EqualTo(dueDate));
        Assert.That(todo.DueDate.Value.Kind, Is.EqualTo(DateTimeKind.Utc));
        Assert.That(todo.Completed, Is.False);
        Assert.That(todo.LastEventNumber, Is.EqualTo(1));
        Assert.That(todo.LastUpdatedAt, Is.GreaterThan(before).And.LessThan(after));
    }

    [Test]
    public async Task TodoController_UpdateTodoDueDate_To_Null_When_It_Is_Already_Null_Returns_Todo_With_Null_DueDate()
    {
        var createResp = await _client.CreateTodoAsync();
        var dueDate = DateTime.UtcNow;

        var before = DateTime.UtcNow;
        var resp = (await _client.UpdateTodoDueDateAsync(createResp.Id, createResp.LastEventNumber, null)).Response;
        var after = DateTime.UtcNow;

        await Task.Delay(TimeSpan.FromSeconds(1));

        var todo = (await _client.GetAllAsync()).First();

        Assert.That(resp.Id, Is.EqualTo(createResp.Id));
        Assert.That(resp.LastEventNumber, Is.EqualTo(1));

        Assert.That(todo.Id, Is.EqualTo(createResp.Id));
        Assert.That(todo.Name, Is.Null);
        Assert.That(todo.Description, Is.Null);
        Assert.That(todo.DueDate, Is.Null);
        Assert.That(todo.Completed, Is.False);
        Assert.That(todo.LastEventNumber, Is.EqualTo(1));
        Assert.That(todo.LastUpdatedAt, Is.GreaterThan(before).And.LessThan(after));
    }

    [Test]
    public async Task TodoController_UpdateTodoDueDate_That_Is_Not_Utc_Returns_DueDate_In_UTC()
    {
        var createResp = await _client.CreateTodoAsync();
        var dueDate = DateTime.Now;

        var before = DateTime.UtcNow;
        var resp = (await _client.UpdateTodoDueDateAsync(createResp.Id, createResp.LastEventNumber, dueDate)).Response;
        var after = DateTime.UtcNow;

        await Task.Delay(TimeSpan.FromSeconds(1));

        var todo = (await _client.GetAllAsync()).First();

        Assert.That(resp.Id, Is.EqualTo(createResp.Id));
        Assert.That(resp.LastEventNumber, Is.EqualTo(1));

        Assert.That(todo.Id, Is.EqualTo(createResp.Id));
        Assert.That(todo.Name, Is.Null);
        Assert.That(todo.Description, Is.Null);
        Assert.That(todo.DueDate.Value, Is.EqualTo(dueDate.ToUniversalTime()));
        Assert.That(todo.Completed, Is.False);
        Assert.That(todo.LastEventNumber, Is.EqualTo(1));
        Assert.That(todo.LastUpdatedAt, Is.GreaterThan(before).And.LessThan(after));
    }

    [Test]
    public async Task TodoController_UpdateTodoDueDate_Whose_Kind_Is_Not_Specified_Returns_DueDate_In_UTC()
    {
        var createResp = await _client.CreateTodoAsync();
        var dueDate = DateTime.Now;
        var dueDateAsUnspecifiedKind = DateTime.SpecifyKind(dueDate, DateTimeKind.Unspecified);
        var dueDateAsUtcKind = DateTime.SpecifyKind(dueDate, DateTimeKind.Utc);

        var before = DateTime.UtcNow;
        var resp = (await _client.UpdateTodoDueDateAsync(createResp.Id, createResp.LastEventNumber, dueDateAsUnspecifiedKind)).Response;
        var after = DateTime.UtcNow;

        await Task.Delay(TimeSpan.FromSeconds(1));

        var todo = (await _client.GetAllAsync()).First();

        Assert.That(resp.Id, Is.EqualTo(createResp.Id));
        Assert.That(resp.LastEventNumber, Is.EqualTo(1));

        Assert.That(todo.Id, Is.EqualTo(createResp.Id));
        Assert.That(todo.Name, Is.Null);
        Assert.That(todo.Description, Is.Null);
        Assert.That(todo.DueDate.Value, Is.EqualTo(dueDateAsUtcKind));
        Assert.That(todo.Completed, Is.False);
        Assert.That(todo.LastEventNumber, Is.EqualTo(1));
        Assert.That(todo.LastUpdatedAt, Is.GreaterThan(before).And.LessThan(after));
    }

    [Test]
    public async Task TodoController_UpdateTodoDueDate_To_Something_And_Then_Back_To_Nothing_Returns_Todo_With_Null_DueDate()
    {
        var createResp = await _client.CreateTodoAsync();
        var dueDate = DateTime.UtcNow;

        var before1 = DateTime.UtcNow;
        var resp1 = (await _client.UpdateTodoDueDateAsync(createResp.Id, createResp.LastEventNumber, dueDate)).Response;
        var after1 = DateTime.UtcNow;

        await Task.Delay(TimeSpan.FromSeconds(1));

        var todo1 = (await _client.GetAllAsync()).First();

        var before2 = DateTime.UtcNow;
        var resp2 = (await _client.UpdateTodoDueDateAsync(createResp.Id, resp1.LastEventNumber, null)).Response;
        var after2 = DateTime.UtcNow;

        await Task.Delay(TimeSpan.FromSeconds(1));

        var todo2 = (await _client.GetAllAsync()).First();

        Assert.That(resp1.Id, Is.EqualTo(createResp.Id));
        Assert.That(resp1.LastEventNumber, Is.EqualTo(1));

        Assert.That(todo1.Id, Is.EqualTo(createResp.Id));
        Assert.That(todo1.Name, Is.Null);
        Assert.That(todo1.Description, Is.Null);
        Assert.That(todo1.DueDate, Is.EqualTo(dueDate));
        Assert.That(todo1.DueDate.Value.Kind, Is.EqualTo(DateTimeKind.Utc));
        Assert.That(todo1.Completed, Is.False);
        Assert.That(todo1.LastEventNumber, Is.EqualTo(1));
        Assert.That(todo1.LastUpdatedAt, Is.GreaterThan(before1).And.LessThan(after1));


        Assert.That(resp2.Id, Is.EqualTo(createResp.Id));
        Assert.That(resp2.LastEventNumber, Is.EqualTo(2));

        Assert.That(todo2.Id, Is.EqualTo(createResp.Id));
        Assert.That(todo2.Name, Is.Null);
        Assert.That(todo2.Description, Is.Null);
        Assert.That(todo2.DueDate, Is.Null);
        Assert.That(todo2.Completed, Is.False);
        Assert.That(todo2.LastEventNumber, Is.EqualTo(2));
        Assert.That(todo2.LastUpdatedAt, Is.GreaterThan(before2).And.LessThan(after2));
    }
}