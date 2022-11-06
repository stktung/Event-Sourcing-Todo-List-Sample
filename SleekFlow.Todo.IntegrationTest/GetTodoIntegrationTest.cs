using System.Net;

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
}