using System.Net;
using Newtonsoft.Json;
using SleekFlow.Todo.Application.Controllers;

namespace SleekFlow.Todo.IntegrationTest;

public class CreateTodoIntegrationTest : TodoIntegrationTest
{
    [Test]
    public async Task TodoController_Simple_Create_Assert_Response_Has_Id_And_EventNumber()
    {

        var createResponse = await _client.CreateTodoAsync();

        Assert.That(createResponse.Id, Is.Not.EqualTo(default(Guid)));
        Assert.That(createResponse.LastEventNumber, Is.EqualTo(0));
    }


    [Test]
    public async Task TodoController_Simple_Create_Get_Assert_Todo_Is_Saved_And_Can_Get_Same_Todo_With_Expected_Data()
    {
        var beforeRequest = DateTime.Now.ToUniversalTime();
        var createResponse = await _client.CreateTodoAsync();
        var afterRequest = DateTime.Now.ToUniversalTime();

        var getResponseBody =
            await (await _client.GetAsync($"/Todo/{createResponse.Id}")).Content.ReadAsStringAsync();
        var getResponse =
            JsonConvert.DeserializeObject<GetTodoResponse>(getResponseBody);

        Assert.That(getResponse.Id, Is.EqualTo(createResponse.Id));
        Assert.That(getResponse.Name, Is.Null);
        Assert.That(getResponse.Description, Is.Null);
        Assert.That(getResponse.DueDate, Is.Null);
        Assert.That(getResponse.Completed, Is.False);
        Assert.That(getResponse.LastEventNumber, Is.Zero);
        Assert.That(getResponse.LastUpdatedAt, Is.GreaterThan(beforeRequest).And.LessThan(afterRequest));
    }

    [Test]
    public async Task TodoController_Create_Few_Todo_And_Call_GetAll_Returns_The_Todos()
    {
        var createResponse1 = await _client.CreateTodoAsync();
        var createResponse2 = await _client.CreateTodoAsync();
        var createResponse3 = await _client.CreateTodoAsync();

        await Task.Delay(TimeSpan.FromSeconds(1));

        var todos = await _client.GetAllAsync();

        var todo1 = todos.First(todo => todo.Id == createResponse1.Id);
        var todo2 = todos.First(todo => todo.Id == createResponse2.Id);
        var todo3 = todos.First(todo => todo.Id == createResponse3.Id);

        Assert.That(todos.Count(), Is.EqualTo(3));

        Assert.That(todo1.Id, Is.EqualTo(createResponse1.Id));
        Assert.That(todo1.Name, Is.Null);
        Assert.That(todo1.Description, Is.Null);
        Assert.That(todo1.DueDate, Is.Null);
        Assert.That(todo1.Completed, Is.False);
        Assert.That(todo1.LastEventNumber, Is.Zero);

        Assert.That(todo2.Id, Is.EqualTo(createResponse2.Id));
        Assert.That(todo2.Name, Is.Null);
        Assert.That(todo2.Description, Is.Null);
        Assert.That(todo2.DueDate, Is.Null);
        Assert.That(todo2.Completed, Is.False);
        Assert.That(todo2.LastEventNumber, Is.Zero);

        Assert.That(todo3.Id, Is.EqualTo(createResponse3.Id));
        Assert.That(todo3.Name, Is.Null);
        Assert.That(todo3.Description, Is.Null);
        Assert.That(todo3.DueDate, Is.Null);
        Assert.That(todo3.Completed, Is.False);
        Assert.That(todo3.LastEventNumber, Is.Zero);
    }

    [Test]
    public async Task TodoController_Create_Many_Todo_And_Call_GetAll_Returns_The_Todos()
    {
        for (int i = 0; i < 100; i++)
        {
            JsonConvert.DeserializeObject<GeneralPostTodoResponse>(await (await _client.PostAsync("/Todo/create", null))
                .Content.ReadAsStringAsync());
        }

        await Task.Delay(TimeSpan.FromSeconds(1));

        var getResponse = await _client.GetAsync($"/Todo");

        Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var getResponseBody =
            await getResponse.Content.ReadAsStringAsync();
        var todos =
            JsonConvert.DeserializeObject<IEnumerable<GetTodoResponse>>(getResponseBody);

        Assert.That(todos.Count(), Is.EqualTo(100));
    }
}