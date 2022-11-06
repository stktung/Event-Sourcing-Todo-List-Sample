using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using SleekFlow.Todo.Application.Controllers;
using SleekFlow.Todo.Application.Middleware;
using SleekFlow.Todo.Domain.Common;

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

            var createResponse = await CreateTodoAsync();
            
            Assert.That(createResponse.Id, Is.Not.EqualTo(default(Guid)));
            Assert.That(createResponse.LastEventNumber, Is.EqualTo(0));
        }


        [Test]
        public async Task TodoController_Simple_Create_Get_Assert_Todo_Is_Saved_And_Can_Get_Same_Todo_With_Expected_Data()
        {
            var beforeRequest = DateTime.Now.ToUniversalTime();
            var createResponse = await CreateTodoAsync();
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
            var createResponse1 = await CreateTodoAsync();
            var createResponse2 = await CreateTodoAsync();
            var createResponse3 = await CreateTodoAsync();

            await Task.Delay(TimeSpan.FromSeconds(1));
            
            var todos = await GetAllAsync();

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

        [Test]
        public async Task TodoController_GetAll_Of_Nothing_Returns_404()
        {
            var getResponse = await _client.GetAsync($"/Todo");

            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task TodoController_InsertTodoNameText_Simple_Returns_Todo_With_Text()
        {
            var createResp = await CreateTodoAsync();
            
            var before = DateTime.UtcNow;
            var insertResp = (await InsertTodoNameTextAsync(createResp.Id, createResp.LastEventNumber, "Testing!", 0)).Response;
            var after = DateTime.UtcNow;
            
            await Task.Delay(TimeSpan.FromSeconds(1));

            var todo = (await GetAllAsync()).First();

            Assert.That(insertResp.Id, Is.EqualTo(createResp.Id));
            Assert.That(insertResp.LastEventNumber, Is.EqualTo(1));

            Assert.That(todo.Id, Is.EqualTo(createResp.Id));
            Assert.That(todo.Name, Is.EqualTo("Testing!"));
            Assert.That(todo.Description, Is.Null);
            Assert.That(todo.DueDate, Is.Null);
            Assert.That(todo.Completed, Is.False);
            Assert.That(todo.LastEventNumber, Is.EqualTo(1));
            Assert.That(todo.LastUpdatedAt, Is.GreaterThan(before).And.LessThan(after));
            
        }

        [Test]
        public async Task TodoController_InsertTodoNameText_Few_Times_Returns_Todo_With_Correct_Text()
        {
            var createResp = await CreateTodoAsync();
            var insertResp1 = (await InsertTodoNameTextAsync(createResp.Id, createResp.LastEventNumber, "testing!", 0)).Response;
            var insertResp2 = (await InsertTodoNameTextAsync(createResp.Id, insertResp1.LastEventNumber, "I'm ", 0)).Response;
            var insertResp3 =
                (await InsertTodoNameTextAsync(createResp.Id, insertResp2.LastEventNumber, "doing some ", 4)).Response;
            var insertResp4 =
                (await InsertTodoNameTextAsync(createResp.Id, insertResp3.LastEventNumber, " Good work!", 23)).Response;

            await Task.Delay(TimeSpan.FromSeconds(1));
;
            var todo = (await GetAllAsync()).First();
            
            Assert.That(insertResp1.Id, Is.EqualTo(createResp.Id));
            Assert.That(insertResp1.LastEventNumber, Is.EqualTo(1));

            Assert.That(insertResp2.Id, Is.EqualTo(createResp.Id));
            Assert.That(insertResp2.LastEventNumber, Is.EqualTo(2));

            Assert.That(insertResp3.Id, Is.EqualTo(createResp.Id));
            Assert.That(insertResp3.LastEventNumber, Is.EqualTo(3));

            Assert.That(insertResp4.Id, Is.EqualTo(createResp.Id));
            Assert.That(insertResp4.LastEventNumber, Is.EqualTo(4));

            Assert.That(todo.Id, Is.EqualTo(createResp.Id));
            Assert.That(todo.Name, Is.EqualTo("I'm doing some testing! Good work!"));
            Assert.That(todo.Description, Is.Null);
            Assert.That(todo.DueDate, Is.Null);
            Assert.That(todo.Completed, Is.False);
            Assert.That(todo.LastEventNumber, Is.EqualTo(4));
        }

        [Test]
        public async Task TodoController_InsertTodoNameText_With_Unknown_Id_Returns_404()
        {
            var insertResp = await InsertTodoNameTextAsync(Guid.NewGuid(), 0, "Testing", 0);
            
            Assert.That(insertResp.HttpResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task TodoController_InsertTodoNameText_With_Missing_Body_Returns_400()
        {
            var insertResp = await InsertTodoNameTextAsync(Guid.NewGuid(), 0, string.Empty, 0, string.Empty);

            Assert.That(insertResp.HttpResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task TodoController_InsertTodoNameText_With_Mismatched_Version_Returns_400()
        {
            var createResp = await CreateTodoAsync();
            var insertResp1 = await InsertTodoNameTextAsync(createResp.Id, -1, string.Empty, 0);
            var insertResp2 = await InsertTodoNameTextAsync(createResp.Id, 1, string.Empty, 0);

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
        public async Task TodoController_InsertTodoNameText_With_Invalid_Position_Returns_400()
        {
            var createResp = await CreateTodoAsync();
            var insertResp1 = await InsertTodoNameTextAsync(createResp.Id, 0, string.Empty, -1);
            var insertResp2 = await InsertTodoNameTextAsync(createResp.Id, 0, string.Empty, 1);

            Assert.That(insertResp1.HttpResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(insertResp1.ErrorResponse!.ErrorType, Is.EqualTo(nameof(DomainException)));
            Assert.That(insertResp1.ErrorResponse!.Message, Is.Not.Null);
            Assert.That(insertResp1.ErrorResponse!.Exception, Is.Not.Null);

            Assert.That(insertResp2.HttpResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(insertResp2.ErrorResponse!.ErrorType, Is.EqualTo(nameof(DomainException)));
            Assert.That(insertResp2.ErrorResponse!.Message, Is.Not.Null);
            Assert.That(insertResp2.ErrorResponse!.Exception, Is.Not.Null);
        }

        private async Task<IEnumerable<GetTodoResponse>> GetAllAsync()
        {
            var getResp = await _client.GetAsync($"/Todo");

            var getResponseBody =
                await getResp.Content.ReadAsStringAsync();
            var todos = JsonConvert.DeserializeObject<IEnumerable<GetTodoResponse>>(getResponseBody);
            return todos;
        }

        private async Task<GeneralPostTodoResponse> CreateTodoAsync()
        {
            return JsonConvert.DeserializeObject<GeneralPostTodoResponse>(
                await (await _client.PostAsync("/Todo/create", null))
                    .Content.ReadAsStringAsync());
        }

        private async Task<(GeneralPostTodoResponse? Response, ErrorResponse? ErrorResponse, HttpResponseMessage HttpResponse)>
            InsertTodoNameTextAsync(Guid id, long expectedVersion, string text, int position, string? contentOverride = null)
        {
            var content = JsonConvert.SerializeObject(new InsertTodoNameTextRequest(
                expectedVersion, text, position));

            if (contentOverride != null) content = contentOverride;

            var insertRespMsg =
                await _client.PostAsync($"/Todo/{id}/name/inserttext",
                    new StringContent(content, Encoding.UTF8, "application/json"));

            GeneralPostTodoResponse resp = null;
            ErrorResponse errResp = null;
            if (insertRespMsg.IsSuccessStatusCode)
            {
                resp = JsonConvert.DeserializeObject<GeneralPostTodoResponse>(await insertRespMsg.Content
                    .ReadAsStringAsync());
            }
            else
            {
                errResp = JsonConvert.DeserializeObject<ErrorResponse>(await insertRespMsg.Content
                    .ReadAsStringAsync());
            }


            return (resp, errResp, insertRespMsg);
        }
    }
}