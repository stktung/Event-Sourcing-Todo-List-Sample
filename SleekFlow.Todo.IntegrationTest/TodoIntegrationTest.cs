using Microsoft.AspNetCore.Mvc.Testing;

namespace SleekFlow.Todo.IntegrationTest
{
    public class TodoIntegrationTest
    {
        protected HttpClient _client;
        private WebApplicationFactory<Program> _webAppFactory;

        [SetUp]
        public void Setup()
        {
            Environment.SetEnvironmentVariable("EVENTSTORE_USERNAME", "admin");
            Environment.SetEnvironmentVariable("EVENTSTORE_PASSWORD", "changeit");
            _webAppFactory = new WebApplicationFactory<Program>();
            _client = _webAppFactory.CreateDefaultClient();
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _webAppFactory.Dispose();
        }
    }
}