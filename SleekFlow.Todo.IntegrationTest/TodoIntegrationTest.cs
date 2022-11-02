using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using SleekFlow.Todo.Application.Controllers;

namespace SleekFlow.Todo.IntegrationTest
{
    public class TodoIntegrationTest
    {
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            var webAppFactory = new WebApplicationFactory<Program>();
            _client = webAppFactory.CreateDefaultClient();
        }

        [Test]
        public async Task TodoController_Simple_Create_Assert_Response_Has_Id_And_EventNumber()
        {

            var createResponse =
                JsonConvert.DeserializeObject<CreateTodoResponse>(await (await _client.PostAsync("/Todo/create", null))
                    .Content.ReadAsStringAsync());
            
            Assert.That(createResponse.Id, Is.Not.EqualTo(default(Guid)));
            Assert.That(createResponse.LastEventNumber, Is.EqualTo(0));
        }


        [Test]
        public async Task TodoController_Simple_Create_Get_Assert_Todo_Is_Saved_And_Can_Get_Same_Todo_With_Expected_Data()
        {
            var beforeRequest = DateTime.Now.ToUniversalTime();
            var createResponse =
                JsonConvert.DeserializeObject<CreateTodoResponse>(await (await _client.PostAsync("/Todo/create", null))
                    .Content.ReadAsStringAsync());
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

    }
}