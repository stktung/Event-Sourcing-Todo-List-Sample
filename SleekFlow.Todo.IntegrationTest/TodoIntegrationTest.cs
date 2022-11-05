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
            Environment.SetEnvironmentVariable("EVENTSTORE_USERNAME", "admin");
            Environment.SetEnvironmentVariable("EVENTSTORE_PASSWORD", "changeit");
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

        [Test]
        public async Task TodoController_Create_Few_Todo_And_Call_GetAll_Returns_The_Todos()
        {
            var createResponse1 =
                JsonConvert.DeserializeObject<CreateTodoResponse>(await (await _client.PostAsync("/Todo/create", null))
                    .Content.ReadAsStringAsync());

            var createResponse2 =
                JsonConvert.DeserializeObject<CreateTodoResponse>(await (await _client.PostAsync("/Todo/create", null))
                    .Content.ReadAsStringAsync());

            var createResponse3 =
                JsonConvert.DeserializeObject<CreateTodoResponse>(await (await _client.PostAsync("/Todo/create", null))
                    .Content.ReadAsStringAsync());

            await Task.Delay(TimeSpan.FromSeconds(1));

            var getResponse = await _client.GetAsync($"/Todo");
            
            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            
            var getResponseBody =
                await getResponse.Content.ReadAsStringAsync();
            var todos =
                JsonConvert.DeserializeObject<IEnumerable<GetTodoResponse>>(getResponseBody);

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
                JsonConvert.DeserializeObject<CreateTodoResponse>(await (await _client.PostAsync("/Todo/create", null))
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

        [Test]
        public async Task TodoController_GetAll_Of_Nothing_Returns_404()
        {
            var getResponse = await _client.GetAsync($"/Todo");

            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

    }
}