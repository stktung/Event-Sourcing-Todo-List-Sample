using System.Net;
using SleekFlow.Todo.Domain.Common;

namespace SleekFlow.Todo.IntegrationTest;

public class InsertTodoDescriptionTextIntegrationTest : TodoIntegrationTest
{
    [Test]
    public async Task TodoController_InsertTodoDescriptionText_Simple_Returns_Todo_With_Text()
    {
        var createResp = await _client.CreateTodoAsync();

        var before = DateTime.UtcNow;
        var insertResp = (await _client.InsertTodoDescriptionTextAsync(createResp.Id, createResp.LastEventNumber, "Testing!", 0)).Response;
        var after = DateTime.UtcNow;

        await Task.Delay(TimeSpan.FromSeconds(1));

        var todo = (await _client.GetAllAsync()).First();

        Assert.That(insertResp.Id, Is.EqualTo(createResp.Id));
        Assert.That(insertResp.LastEventNumber, Is.EqualTo(1));

        Assert.That(todo.Id, Is.EqualTo(createResp.Id));
        Assert.That(todo.Name, Is.Null);
        Assert.That(todo.Description, Is.EqualTo("Testing!"));
        Assert.That(todo.DueDate, Is.Null);
        Assert.That(todo.Completed, Is.False);
        Assert.That(todo.LastEventNumber, Is.EqualTo(1));
        Assert.That(todo.LastUpdatedAt, Is.GreaterThan(before).And.LessThan(after));

    }

    [Test]
    public async Task TodoController_InsertTodoDescriptionText_Few_Times_Returns_Todo_With_Correct_Text()
    {
        var createResp = await _client.CreateTodoAsync();
        var insertResp1 = (await _client.InsertTodoDescriptionTextAsync(createResp.Id, createResp.LastEventNumber, "testing!", 0)).Response;
        var insertResp2 = (await _client.InsertTodoDescriptionTextAsync(createResp.Id, insertResp1.LastEventNumber, "I'm ", 0)).Response;
        var insertResp3 =
            (await _client.InsertTodoDescriptionTextAsync(createResp.Id, insertResp2.LastEventNumber, "doing some ", 4)).Response;
        var insertResp4 =
            (await _client.InsertTodoDescriptionTextAsync(createResp.Id, insertResp3.LastEventNumber, " Good work!", 23)).Response;

        await Task.Delay(TimeSpan.FromSeconds(1));
        ;
        var todo = (await _client.GetAllAsync()).First();

        Assert.That(insertResp1.Id, Is.EqualTo(createResp.Id));
        Assert.That(insertResp1.LastEventNumber, Is.EqualTo(1));

        Assert.That(insertResp2.Id, Is.EqualTo(createResp.Id));
        Assert.That(insertResp2.LastEventNumber, Is.EqualTo(2));

        Assert.That(insertResp3.Id, Is.EqualTo(createResp.Id));
        Assert.That(insertResp3.LastEventNumber, Is.EqualTo(3));

        Assert.That(insertResp4.Id, Is.EqualTo(createResp.Id));
        Assert.That(insertResp4.LastEventNumber, Is.EqualTo(4));

        Assert.That(todo.Id, Is.EqualTo(createResp.Id));
        Assert.That(todo.Name, Is.Null);
        Assert.That(todo.Description, Is.EqualTo("I'm doing some testing! Good work!"));
        Assert.That(todo.DueDate, Is.Null);
        Assert.That(todo.Completed, Is.False);
        Assert.That(todo.LastEventNumber, Is.EqualTo(4));
    }

    [Test]
    public async Task TodoController_InsertTodoDescriptionText_With_Unknown_Id_Returns_404()
    {
        var insertResp = await _client.InsertTodoDescriptionTextAsync(Guid.NewGuid(), 0, "Testing", 0);

        Assert.That(insertResp.HttpResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task TodoController_InsertTodoDescriptionText_With_Missing_Body_Returns_400()
    {
        var insertResp = await _client.InsertTodoDescriptionTextAsync(Guid.NewGuid(), 0, string.Empty, 0, string.Empty);

        Assert.That(insertResp.HttpResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task TodoController_InsertTodoDescriptionText_With_Mismatched_Version_Returns_400()
    {
        var createResp = await _client.CreateTodoAsync();
        var insertResp1 = await _client.InsertTodoDescriptionTextAsync(createResp.Id, -1, string.Empty, 0);
        var insertResp2 = await _client.InsertTodoDescriptionTextAsync(createResp.Id, 1, string.Empty, 0);

        Assert.That(insertResp1.HttpResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(insertResp1.ErrorResponse!.ErrorType, Is.EqualTo(nameof(AggregateWrongExpectedVersionException)));
        Assert.That(insertResp1.ErrorResponse!.Message, Is.Not.Null);
        Assert.That(insertResp1.ErrorResponse!.Exception, Is.Not.Null);

        Assert.That(insertResp2.HttpResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(insertResp2.ErrorResponse!.ErrorType, Is.EqualTo(nameof(AggregateWrongExpectedVersionException)));
        Assert.That(insertResp2.ErrorResponse!.Message, Is.Not.Null);
        Assert.That(insertResp2.ErrorResponse!.Exception, Is.Not.Null);
    }

    [Test]
    public async Task TodoController_InsertTodoDescriptionText_With_Invalid_Position_Returns_400()
    {
        var createResp = await _client.CreateTodoAsync();
        var insertResp1 = await _client.InsertTodoDescriptionTextAsync(createResp.Id, 0, string.Empty, -1);
        var insertResp2 = await _client.InsertTodoDescriptionTextAsync(createResp.Id, 0, string.Empty, 1);

        Assert.That(insertResp1.HttpResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(insertResp1.ErrorResponse!.ErrorType, Is.EqualTo(nameof(DomainException)));
        Assert.That(insertResp1.ErrorResponse!.Message, Is.Not.Null);
        Assert.That(insertResp1.ErrorResponse!.Exception, Is.Not.Null);

        Assert.That(insertResp2.HttpResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(insertResp2.ErrorResponse!.ErrorType, Is.EqualTo(nameof(DomainException)));
        Assert.That(insertResp2.ErrorResponse!.Message, Is.Not.Null);
        Assert.That(insertResp2.ErrorResponse!.Exception, Is.Not.Null);
    }
}