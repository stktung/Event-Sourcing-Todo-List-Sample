using System.Net;
using SleekFlow.Todo.Application.Controllers;
using SleekFlow.Todo.Application.Model.Event;
using SleekFlow.Todo.Domain.Event;

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

    [Test]
    public async Task TodoController_GetAll_With_DueDate_Sorting_Desc_Returns_Todos_Sorted_By_DueDate_DescXXXX()
    {
        var resp1 = await _client.CreateTodoAsync();
        var dueDate = DateTime.UtcNow.AddDays(10);
        var resp2 = (await _client.UpdateTodoDueDateAsync(resp1.Id, resp1.LastEventNumber, dueDate)).Response;
        var resp3 = (await _client.InsertTodoNameTextAsync(resp1.Id, resp2.LastEventNumber, "Testing!", 0)).Response;
        var resp4 = (await _client.DeleteTodoNameTextAsync(resp1.Id, resp3.LastEventNumber, 7, 1)).Response;
        var resp5 = (await _client.InsertTodoDescriptionTextAsync(resp1.Id, resp4.LastEventNumber, "Testing Desc!", 0)).Response;
        var resp6 = (await _client.DeleteTodoDescriptionTextAsync(resp1.Id, resp5.LastEventNumber, 12, 1)).Response;
        var resp7 = (await _client.UpdateTodoIsCompletedAsync(resp1.Id, resp6.LastEventNumber, true)).Response;
        var resp8 = (await _client.UpdateTodoIsCompletedAsync(resp1.Id, resp7.LastEventNumber, false)).Response;


        var getResp = (await _client.GetHistoryAsync(resp1.Id)).ToList();

        var evt1 = (getResp[0] as TodoCreatedEventWebDto);
        var evt2 = (getResp[1] as TodoDueDateUpdatedEventWebDto);
        var evt3 = (getResp[2] as TodoNameTextInsertedEventWebDto);
        var evt4 = (getResp[3] as TodoNameTextDeletedEventWebDto);
        var evt5 = (getResp[4] as TodoDescriptionTextInsertedEventWebDto);
        var evt6 = (getResp[5] as TodoDescriptionTextDeletedEventWebDto);
        var evt7 = (getResp[6] as TodoCompleteMarkedEventWebDto);
        var evt8 = (getResp[7] as TodoCompleteUnmarkedEventWebDto);
        
        Assert.That(evt1, Is.Not.Null);
        Assert.That(evt1.Id, Is.EqualTo(resp1.Id));
        Assert.That(evt1.EventNumber, Is.EqualTo(0));
        Assert.That(evt1.EventType, Is.EqualTo(nameof(TodoCreatedEvent)));

        Assert.That(evt2, Is.Not.Null);
        Assert.That(evt2.Id, Is.EqualTo(resp1.Id));
        Assert.That(evt2.EventNumber, Is.EqualTo(1));
        Assert.That(evt2.EventType, Is.EqualTo(nameof(TodoDueDateUpdatedEvent)));
        Assert.That(evt2.DueDate, Is.EqualTo(dueDate));

        Assert.That(evt3, Is.Not.Null);
        Assert.That(evt3.Id, Is.EqualTo(resp1.Id));
        Assert.That(evt3.EventNumber, Is.EqualTo(2));
        Assert.That(evt3.EventType, Is.EqualTo(nameof(TodoNameTextInsertedEvent)));
        Assert.That(evt3.Text, Is.EqualTo("Testing!"));
        Assert.That(evt3.Position, Is.EqualTo(0));

        Assert.That(evt4, Is.Not.Null);
        Assert.That(evt4.Id, Is.EqualTo(resp1.Id));
        Assert.That(evt4.EventNumber, Is.EqualTo(3));
        Assert.That(evt4.EventType, Is.EqualTo(nameof(TodoNameTextDeletedEvent)));
        Assert.That(evt4.Position, Is.EqualTo(7));
        Assert.That(evt4.Length, Is.EqualTo(1));

        Assert.That(evt5, Is.Not.Null);
        Assert.That(evt5.Id, Is.EqualTo(resp1.Id));
        Assert.That(evt5.EventNumber, Is.EqualTo(4));
        Assert.That(evt5.EventType, Is.EqualTo(nameof(TodoDescriptionTextInsertedEvent)));
        Assert.That(evt5.Text, Is.EqualTo("Testing Desc!"));
        Assert.That(evt5.Position, Is.EqualTo(0));

        Assert.That(evt6, Is.Not.Null);
        Assert.That(evt6.Id, Is.EqualTo(resp1.Id));
        Assert.That(evt6.EventNumber, Is.EqualTo(5));
        Assert.That(evt6.EventType, Is.EqualTo(nameof(TodoDescriptionTextDeletedEvent)));
        Assert.That(evt6.Position, Is.EqualTo(12));
        Assert.That(evt6.Length, Is.EqualTo(1));

        Assert.That(evt7, Is.Not.Null);
        Assert.That(evt7.Id, Is.EqualTo(resp1.Id));
        Assert.That(evt7.EventNumber, Is.EqualTo(6));
        Assert.That(evt7.EventType, Is.EqualTo(nameof(TodoCompleteMarkedEvent)));

        Assert.That(evt8, Is.Not.Null);
        Assert.That(evt8.Id, Is.EqualTo(resp1.Id));
        Assert.That(evt8.EventNumber, Is.EqualTo(7));
        Assert.That(evt8.EventType, Is.EqualTo(nameof(TodoCompleteUnmarkedEvent)));
    }
}