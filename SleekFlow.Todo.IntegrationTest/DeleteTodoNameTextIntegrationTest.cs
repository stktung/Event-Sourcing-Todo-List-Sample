using System.Net;
using SleekFlow.Todo.Domain.Common;

namespace SleekFlow.Todo.IntegrationTest;

public class DeleteTodoNameTextIntegrationTest : TodoIntegrationTest
{
    [Test]
    public async Task TodoController_DeleteTodoNameText_Simple_Returns_Todo_With_Text_After_Deletion()
    {
        var createResp = await _client.CreateTodoAsync();

        var insertResp = (await _client.InsertTodoNameTextAsync(createResp.Id, createResp.LastEventNumber, "Testing!", 0)).Response;

        var before1 = DateTime.UtcNow;
        var deleteResp1 = (await _client.DeleteTodoNameTextAsync(createResp.Id, insertResp.LastEventNumber, 4, 3)).Response;
        var after1 = DateTime.UtcNow;

        await Task.Delay(TimeSpan.FromSeconds(1));

        var todo1 = (await _client.GetAllAsync()).First();

        var before2 = DateTime.UtcNow;
        var deleteResp2 = (await _client.DeleteTodoNameTextAsync(createResp.Id, deleteResp1.LastEventNumber, 4, 1)).Response;
        var after2 = DateTime.UtcNow;

        await Task.Delay(TimeSpan.FromSeconds(1));

        var todo2 = (await _client.GetAllAsync()).First();

        var before3 = DateTime.UtcNow;
        var deleteResp3 = (await _client.DeleteTodoNameTextAsync(createResp.Id, deleteResp2.LastEventNumber, 0, 4)).Response;
        var after3 = DateTime.UtcNow;

        await Task.Delay(TimeSpan.FromSeconds(1));

        var todo3 = (await _client.GetAllAsync()).First();


        Assert.That(deleteResp1.Id, Is.EqualTo(createResp.Id));
        Assert.That(deleteResp1.LastEventNumber, Is.EqualTo(2));

        Assert.That(todo1.Id, Is.EqualTo(createResp.Id));
        Assert.That(todo1.Name, Is.EqualTo("Test!"));
        Assert.That(todo1.Description, Is.Null);
        Assert.That(todo1.DueDate, Is.Null);
        Assert.That(todo1.Completed, Is.False);
        Assert.That(todo1.LastEventNumber, Is.EqualTo(2));
        Assert.That(todo1.LastUpdatedAt, Is.GreaterThan(before1).And.LessThan(after1));

        Assert.That(deleteResp2.Id, Is.EqualTo(createResp.Id));
        Assert.That(deleteResp2.LastEventNumber, Is.EqualTo(3));

        Assert.That(todo2.Id, Is.EqualTo(createResp.Id));
        Assert.That(todo2.Name, Is.EqualTo("Test"));
        Assert.That(todo2.Description, Is.Null);
        Assert.That(todo2.DueDate, Is.Null);
        Assert.That(todo2.Completed, Is.False);
        Assert.That(todo2.LastEventNumber, Is.EqualTo(3));
        Assert.That(todo2.LastUpdatedAt, Is.GreaterThan(before2).And.LessThan(after2));

        Assert.That(deleteResp3.Id, Is.EqualTo(createResp.Id));
        Assert.That(deleteResp3.LastEventNumber, Is.EqualTo(4));

        Assert.That(todo3.Id, Is.EqualTo(createResp.Id));
        Assert.That(todo3.Name, Is.EqualTo(string.Empty));
        Assert.That(todo3.Description, Is.Null);
        Assert.That(todo3.DueDate, Is.Null);
        Assert.That(todo3.Completed, Is.False);
        Assert.That(todo3.LastEventNumber, Is.EqualTo(4));
        Assert.That(todo3.LastUpdatedAt, Is.GreaterThan(before3).And.LessThan(after3));

    }

    [Test]
    public async Task TodoController_DeleteTodoNameText_With_Unknown_Id_Returns_404()
    {
        var resp = await _client.DeleteTodoNameTextAsync(Guid.NewGuid(), 0, 0, 0);

        Assert.That(resp.HttpResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task TodoController_DeleteTodoNameText_With_Missing_Body_Returns_400()
    {
        var resp = await _client.DeleteTodoNameTextAsync(Guid.NewGuid(), 0, 0, 0, string.Empty);

        Assert.That(resp.HttpResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task TodoController_DeleteTodoNameText_With_Mismatched_Version_Returns_400()
    {
        var createResp = await _client.CreateTodoAsync();
        var deleteResp1 = await _client.DeleteTodoNameTextAsync(createResp.Id, -1, 0, 0);
        var deleteResp2 = await _client.DeleteTodoNameTextAsync(createResp.Id, 1, 0, 0);

        Assert.That(deleteResp1.HttpResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(deleteResp1.ErrorResponse!.ErrorType, Is.EqualTo(nameof(AggregateWrongExpectedVersionException)));
        Assert.That(deleteResp1.ErrorResponse!.Message, Is.Not.Null);
        Assert.That(deleteResp1.ErrorResponse!.Exception, Is.Not.Null);

        Assert.That(deleteResp2.HttpResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(deleteResp2.ErrorResponse!.ErrorType, Is.EqualTo(nameof(AggregateWrongExpectedVersionException)));
        Assert.That(deleteResp2.ErrorResponse!.Message, Is.Not.Null);
        Assert.That(deleteResp2.ErrorResponse!.Exception, Is.Not.Null);
    }

    [Test]
    public async Task TodoController_DeleteTodoNameText_With_Invalid_Position_Returns_400()
    {
        var createResp = await _client.CreateTodoAsync();
        var insertResp = await _client.InsertTodoNameTextAsync(createResp.Id, createResp.LastEventNumber, "Testing!", 0);
        var deleteResp1 = await _client.DeleteTodoNameTextAsync(createResp.Id, insertResp.Response.LastEventNumber, -1, 0);
        var deleteResp2 = await _client.DeleteTodoNameTextAsync(createResp.Id, insertResp.Response.LastEventNumber, 9, 0);

        Assert.That(deleteResp1.HttpResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(deleteResp1.ErrorResponse!.ErrorType, Is.EqualTo(nameof(DomainException)));
        Assert.That(deleteResp1.ErrorResponse!.Message, Is.Not.Null);
        Assert.That(deleteResp1.ErrorResponse!.Exception, Is.Not.Null);

        Assert.That(deleteResp2.HttpResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(deleteResp2.ErrorResponse!.ErrorType, Is.EqualTo(nameof(DomainException)));
        Assert.That(deleteResp2.ErrorResponse!.Message, Is.Not.Null);
        Assert.That(deleteResp2.ErrorResponse!.Exception, Is.Not.Null);
    }

    [Test]
    public async Task TodoController_DeleteTodoNameText_Beyond_Current_Text_Returns_400()
    {
        var createResp = await _client.CreateTodoAsync();
        var insertResp = await _client.InsertTodoNameTextAsync(createResp.Id, createResp.LastEventNumber, "Testing!", 0);
        var deleteResp1 = await _client.DeleteTodoNameTextAsync(createResp.Id, insertResp.Response.LastEventNumber, 0, 9);

        Assert.That(deleteResp1.HttpResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(deleteResp1.ErrorResponse!.ErrorType, Is.EqualTo(nameof(DomainException)));
        Assert.That(deleteResp1.ErrorResponse!.Message, Is.Not.Null);
        Assert.That(deleteResp1.ErrorResponse!.Exception, Is.Not.Null);
    }
}