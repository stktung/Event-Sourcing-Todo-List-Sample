using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using SleekFlow.Todo.Application.Controllers;

namespace SleekFlow.Todo.IntegrationTest
{
    public class TodoControllerTest
    {
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            var webAppFactory = new WebApplicationFactory<Program>();
            _client = webAppFactory.CreateDefaultClient();
        }

        [Test]
        public async Task TodoController_Simple_Create_Get_Returns_Same_Todo_Id()
        {

            var createResponse =
                JsonConvert.DeserializeObject<CreateTodoResponse>(await (await _client.PostAsync("/Todo/create", null))
                    .Content.ReadAsStringAsync());

            var getResponse =
                JsonConvert.DeserializeObject<GetTodoResponse>(await (await _client.GetAsync($"/Todo/{createResponse.Id}"))
                    .Content.ReadAsStringAsync());
            
            Assert.That(getResponse.Id, Is.EqualTo(createResponse.Id));
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