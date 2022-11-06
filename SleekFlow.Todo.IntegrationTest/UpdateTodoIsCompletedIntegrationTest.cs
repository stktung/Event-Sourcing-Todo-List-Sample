using System.Net;
using SleekFlow.Todo.Domain.Common;

namespace SleekFlow.Todo.IntegrationTest;

public class UpdateTodoIsCompletedIntegrationTest : TodoIntegrationTest
{
    [Test]
    public async Task TodoController_UpdateTodoIsCompleted_Set_To_True_Returns_Completed_Todo()
    {
        var createResp = await _client.CreateTodoAsync();

        var before = DateTime.UtcNow;
        var resp = (await _client.UpdateTodoIsCompletedAsync(createResp.Id, createResp.LastEventNumber, true)).Response;
        var after = DateTime.UtcNow;

        await Task.Delay(TimeSpan.FromSeconds(1));

        var todo = (await _client.GetAllAsync()).First();

        Assert.That(resp.Id, Is.EqualTo(createResp.Id));
        Assert.That(resp.LastEventNumber, Is.EqualTo(1));

        Assert.That(todo.Id, Is.EqualTo(createResp.Id));
        Assert.That(todo.Name, Is.Null);
        Assert.That(todo.Description, Is.Null);
        Assert.That(todo.DueDate, Is.Null);
        Assert.That(todo.Completed, Is.True);
        Assert.That(todo.LastEventNumber, Is.EqualTo(1));
        Assert.That(todo.LastUpdatedAt, Is.GreaterThan(before).And.LessThan(after));
    }

    [Test]
    public async Task TodoController_UpdateTodoIsCompleted_Toggle_Completed_A_Few_Times_Returns_Correct_Completed_State()
    {
        var createResp = await _client.CreateTodoAsync();

        var before1 = DateTime.UtcNow;
        var resp1 = (await _client.UpdateTodoIsCompletedAsync(createResp.Id, createResp.LastEventNumber, true)).Response;
        var after1 = DateTime.UtcNow;

        await Task.Delay(TimeSpan.FromSeconds(1));

        var todo1 = (await _client.GetAllAsync()).First();

        Assert.That(resp1.Id, Is.EqualTo(createResp.Id));
        Assert.That(resp1.LastEventNumber, Is.EqualTo(1));

        Assert.That(todo1.Id, Is.EqualTo(createResp.Id));
        Assert.That(todo1.Name, Is.Null);
        Assert.That(todo1.Description, Is.Null);
        Assert.That(todo1.DueDate, Is.Null);
        Assert.That(todo1.Completed, Is.True);
        Assert.That(todo1.LastEventNumber, Is.EqualTo(1));
        Assert.That(todo1.LastUpdatedAt, Is.GreaterThan(before1).And.LessThan(after1));


        var before2 = DateTime.UtcNow;
        var resp2 = (await _client.UpdateTodoIsCompletedAsync(createResp.Id, resp1.LastEventNumber, false)).Response;
        var after2 = DateTime.UtcNow;

        await Task.Delay(TimeSpan.FromSeconds(1));

        var todo2 = (await _client.GetAllAsync()).First();

        Assert.That(resp2.Id, Is.EqualTo(createResp.Id));
        Assert.That(resp2.LastEventNumber, Is.EqualTo(2));

        Assert.That(todo2.Id, Is.EqualTo(createResp.Id));
        Assert.That(todo2.Name, Is.Null);
        Assert.That(todo2.Description, Is.Null);
        Assert.That(todo2.DueDate, Is.Null);
        Assert.That(todo2.Completed, Is.False);
        Assert.That(todo2.LastEventNumber, Is.EqualTo(2));
        Assert.That(todo2.LastUpdatedAt, Is.GreaterThan(before2).And.LessThan(after2));

        var before3 = DateTime.UtcNow;
        var resp3 = (await _client.UpdateTodoIsCompletedAsync(createResp.Id, resp2.LastEventNumber, true)).Response;
        var after3 = DateTime.UtcNow;

        await Task.Delay(TimeSpan.FromSeconds(1));

        var todo3 = (await _client.GetAllAsync()).First();

        Assert.That(resp3.Id, Is.EqualTo(createResp.Id));
        Assert.That(resp3.LastEventNumber, Is.EqualTo(3));

        Assert.That(todo3.Id, Is.EqualTo(createResp.Id));
        Assert.That(todo3.Name, Is.Null);
        Assert.That(todo3.Description, Is.Null);
        Assert.That(todo3.DueDate, Is.Null);
        Assert.That(todo3.Completed, Is.True);
        Assert.That(todo3.LastEventNumber, Is.EqualTo(3));
        Assert.That(todo3.LastUpdatedAt, Is.GreaterThan(before3).And.LessThan(after3));
    }

    [Test]
    public async Task TodoController_UpdateTodoIsCompleted_Call_Various_Update_Methods_When_Todo_Is_Completed_Returns_400()
    {
        var createResp = await _client.CreateTodoAsync();

        var resp1 = (await _client.UpdateTodoIsCompletedAsync(createResp.Id, createResp.LastEventNumber, true)).Response;

        var resp2 = (await _client.InsertTodoNameTextAsync(createResp.Id, resp1.LastEventNumber, "Test", 0));
        var resp3 = (await _client.DeleteTodoNameTextAsync(createResp.Id, resp1.LastEventNumber, 0, 0));
        var resp4 = (await _client.InsertTodoDescriptionTextAsync(createResp.Id, resp1.LastEventNumber, "Test", 0));
        var resp5 = (await _client.DeleteTodoDescriptionTextAsync(createResp.Id, resp1.LastEventNumber, 0, 0));
        var resp6 = (await _client.UpdateTodoDueDateAsync(createResp.Id, resp1.LastEventNumber, DateTime.UtcNow));

        var resp7 = (await _client.UpdateTodoIsCompletedAsync(createResp.Id, resp1.LastEventNumber, false)).Response;

        var resp8 = (await _client.InsertTodoNameTextAsync(createResp.Id, resp7.LastEventNumber, "Test", 0));
        var resp9 = (await _client.DeleteTodoNameTextAsync(createResp.Id, resp8.Response.LastEventNumber, 0, 0));
        var resp10 = (await _client.InsertTodoDescriptionTextAsync(createResp.Id, resp9.Response.LastEventNumber, "Test", 0));
        var resp11 = (await _client.DeleteTodoDescriptionTextAsync(createResp.Id, resp10.Response.LastEventNumber, 0, 0));
        var resp12 = (await _client.UpdateTodoDueDateAsync(createResp.Id, resp11.Response.LastEventNumber, DateTime.UtcNow));


        Assert.That(resp2.HttpResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(resp2.ErrorResponse.ErrorType, Is.EqualTo(nameof(DomainException)));
        Assert.That(resp3.HttpResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(resp3.ErrorResponse.ErrorType, Is.EqualTo(nameof(DomainException)));
        Assert.That(resp4.HttpResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(resp4.ErrorResponse.ErrorType, Is.EqualTo(nameof(DomainException)));
        Assert.That(resp5.HttpResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(resp5.ErrorResponse.ErrorType, Is.EqualTo(nameof(DomainException)));
        Assert.That(resp6.HttpResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(resp6.ErrorResponse.ErrorType, Is.EqualTo(nameof(DomainException)));

        Assert.That(resp8.HttpResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(resp9.HttpResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(resp10.HttpResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(resp11.HttpResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(resp12.HttpResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}