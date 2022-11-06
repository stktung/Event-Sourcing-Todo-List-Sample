using System.Net;
using SleekFlow.Todo.Application.Controllers;

namespace SleekFlow.Todo.IntegrationTest;

public class GetTodoIntegrationTest : TodoIntegrationTest
{
    [Test]
    public async Task TodoController_Get_Non_Existing_Todo_Returns_404()
    {
        var httpResponseMessage1 = await _client.GetAsync($"/Todo");
        var httpResponseMessage2 = await _client.GetAsync($"/Todo/abc");
        var httpResponseMessage3 = await _client.GetAsync($"/Todo/00000000-0000-0000-0000-000000000000");
        var httpResponseMessage4 = await _client.GetAsync($"/Todo/${Guid.NewGuid()}"); // new random guid

        Assert.That(httpResponseMessage1.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        Assert.That(httpResponseMessage2.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        Assert.That(httpResponseMessage3.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        Assert.That(httpResponseMessage4.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task TodoController_GetAll_Of_Nothing_Returns_404()
    {
        var getResponse = await _client.GetAsync($"/Todo");

        Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task TodoController_GetAll_Completed_Returns_Only_Completed_Todos()
    {
        var resp1 = await _client.CreateTodoAsync();
        var resp2 = await _client.CreateTodoAsync();
        var resp3 = await _client.CreateTodoAsync();
        var resp4 = await _client.UpdateTodoIsCompletedAsync(resp1.Id, resp1.LastEventNumber, true);
        var resp5 = await _client.UpdateTodoIsCompletedAsync(resp3.Id, resp3.LastEventNumber, true);

        var getResp = (await _client.GetAllAsync(true)).ToList();
        
        Assert.That(getResp.Count, Is.EqualTo(2));
        Assert.That(getResp[0].Id, Is.EqualTo(resp1.Id));
        Assert.That(getResp[0].Completed, Is.True);
        Assert.That(getResp[1].Completed, Is.True);
        Assert.That(getResp[1].Id, Is.EqualTo(resp3.Id));
    }

    [Test]
    public async Task TodoController_GetAll_With_DueDate_Range_Returns_Only_Todos_Within_The_Range()
    {
        var resp1 = await _client.CreateTodoAsync();
        var resp2 = await _client.CreateTodoAsync();
        var resp3 = await _client.CreateTodoAsync();
        var resp4 = await _client.UpdateTodoDueDateAsync(resp1.Id, resp1.LastEventNumber, DateTime.UtcNow.AddDays(10));
        var resp5 = await _client.UpdateTodoDueDateAsync(resp2.Id, resp2.LastEventNumber, DateTime.UtcNow.AddDays(20));
        var resp6 = await _client.UpdateTodoDueDateAsync(resp3.Id, resp3.LastEventNumber, DateTime.UtcNow.AddDays(30));

        var getResp = (await _client.GetAllAsync(dueDateAfter: DateTime.UtcNow.AddDays(5),
            dueDateBefore: DateTime.UtcNow.AddDays(25))).ToList();
        
        Assert.That(getResp.Count, Is.EqualTo(2));
        Assert.That(getResp[0].Id, Is.EqualTo(resp1.Id));
        Assert.That(getResp[1].Id, Is.EqualTo(resp2.Id));
    }

    [Test]
    public async Task TodoController_GetAll_With_Name_Sorting_Returns_Todos_Sorted_By_Name()
    {
        var resp1 = await _client.CreateTodoAsync();
        var resp2 = await _client.CreateTodoAsync();
        var resp3 = await _client.CreateTodoAsync();
        var resp4 = await _client.InsertTodoNameTextAsync(resp1.Id, resp1.LastEventNumber, "ZZZ", 0);
        var resp5 = await _client.InsertTodoNameTextAsync(resp2.Id, resp2.LastEventNumber, "YYY", 0);
        var resp6 = await _client.InsertTodoNameTextAsync(resp3.Id, resp3.LastEventNumber, "XXX", 0);

        await Task.Delay(TimeSpan.FromSeconds(1));

        var getResp = (await _client.GetAllAsync(sortByField: TodoController.SortByField.Name, sortAsc: true)).ToList();

        Assert.That(getResp[0].Id, Is.EqualTo(resp3.Id));
        Assert.That(getResp[1].Id, Is.EqualTo(resp2.Id));
        Assert.That(getResp[2].Id, Is.EqualTo(resp1.Id));
    }

    [Test]
    public async Task TodoController_GetAll_With_Name_Sorting_Desc_Returns_Todos_Sorted_By_Name_Desc()
    {
        var resp1 = await _client.CreateTodoAsync();
        var resp2 = await _client.CreateTodoAsync();
        var resp3 = await _client.CreateTodoAsync();
        var resp4 = await _client.InsertTodoNameTextAsync(resp1.Id, resp1.LastEventNumber, "XXX", 0);
        var resp5 = await _client.InsertTodoNameTextAsync(resp2.Id, resp2.LastEventNumber, "YYY", 0);
        var resp6 = await _client.InsertTodoNameTextAsync(resp3.Id, resp3.LastEventNumber, "ZZZ", 0);

        await Task.Delay(TimeSpan.FromSeconds(1));

        var getResp = (await _client.GetAllAsync(sortByField: TodoController.SortByField.Name, sortAsc: false)).ToList();

        Assert.That(getResp[0].Id, Is.EqualTo(resp3.Id));
        Assert.That(getResp[1].Id, Is.EqualTo(resp2.Id));
        Assert.That(getResp[2].Id, Is.EqualTo(resp1.Id));
    }

    [Test]
    public async Task TodoController_GetAll_With_DueDate_Sorting_Returns_Todos_Sorted_By_DueDate()
    {
        var resp1 = await _client.CreateTodoAsync();
        var resp2 = await _client.CreateTodoAsync();
        var resp3 = await _client.CreateTodoAsync();
        var resp4 = await _client.UpdateTodoDueDateAsync(resp1.Id, resp1.LastEventNumber, DateTime.UtcNow.AddDays(30));
        var resp5 = await _client.UpdateTodoDueDateAsync(resp2.Id, resp2.LastEventNumber, DateTime.UtcNow.AddDays(20));
        var resp6 = await _client.UpdateTodoDueDateAsync(resp3.Id, resp3.LastEventNumber, DateTime.UtcNow.AddDays(10));

        await Task.Delay(TimeSpan.FromSeconds(1));

        var getResp = (await _client.GetAllAsync(sortByField: TodoController.SortByField.DueDate, sortAsc: true)).ToList();

        Assert.That(getResp[0].Id, Is.EqualTo(resp3.Id));
        Assert.That(getResp[1].Id, Is.EqualTo(resp2.Id));
        Assert.That(getResp[2].Id, Is.EqualTo(resp1.Id));
    }

    [Test]
    public async Task TodoController_GetAll_With_DueDate_Sorting_Desc_Returns_Todos_Sorted_By_DueDate_Desc()
    {
        var resp1 = await _client.CreateTodoAsync();
        var resp2 = await _client.CreateTodoAsync();
        var resp3 = await _client.CreateTodoAsync();
        var resp4 = await _client.UpdateTodoDueDateAsync(resp1.Id, resp1.LastEventNumber, DateTime.UtcNow.AddDays(10));
        var resp5 = await _client.UpdateTodoDueDateAsync(resp2.Id, resp2.LastEventNumber, DateTime.UtcNow.AddDays(20));
        var resp6 = await _client.UpdateTodoDueDateAsync(resp3.Id, resp3.LastEventNumber, DateTime.UtcNow.AddDays(30));

        await Task.Delay(TimeSpan.FromSeconds(1));

        var getResp = (await _client.GetAllAsync(sortByField: TodoController.SortByField.DueDate, sortAsc: false)).ToList();

        Assert.That(getResp[0].Id, Is.EqualTo(resp3.Id));
        Assert.That(getResp[1].Id, Is.EqualTo(resp2.Id));
        Assert.That(getResp[2].Id, Is.EqualTo(resp1.Id));
    }
}