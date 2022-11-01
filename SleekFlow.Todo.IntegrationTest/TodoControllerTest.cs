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
    }
}