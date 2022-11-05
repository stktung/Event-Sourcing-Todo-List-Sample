using System.Text;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Newtonsoft.Json;
using SleekFlow.Todo.Domain;
using SleekFlow.Todo.Domain.Common;
using SleekFlow.Todo.Infrastructure.EmbeddedEventStoreDb;

namespace SleekFlow.Todo.Infrastructure.Test
{
    public class EmbeddedEventStoreTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task Embedded_EventStore_Connect_Append_Read_Simple_Test()
        {
            var db = new EmbeddedEventStoreDb.EmbeddedEventStoreDb();
            
            var expected = "{\"Foo\":\"Bar\"}";

            var streamName = "some-stream";

            await db.Connection.AppendToStreamAsync(
                streamName,
                ExpectedVersion.Any,
                new EventData(Guid.NewGuid(), "eventType", true,
                    Encoding.UTF8.GetBytes(expected), null)
            );


            var eventReadResult = await db.Connection.ReadEventAsync(streamName, 0, false);
            var actual = Encoding.UTF8.GetString(eventReadResult.Event.Value.Event.Data);

            Assert.IsNotNull(eventReadResult);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task Embedded_EventStore_Subscription1234123412341234()
        {
            var db = new EmbeddedEventStoreDb.EmbeddedEventStoreDb();

            var expected = "{\"Foo\":\"Bar\"}";

            var streamName = "some-stream";
            var groupName = "group1";

            
            await db.Connection.CreatePersistentSubscriptionAsync(streamName, groupName,
                PersistentSubscriptionSettings.Create().Build(), new UserCredentials("admin", "changeit"));

            var task = db.Connection.ConnectToPersistentSubscriptionAsync(
                streamName,
                groupName,
                (eventStorePersistentSubscriptionBase, resolvedEvent) =>
                {
                    Console.WriteLine("Hello world");
                    eventStorePersistentSubscriptionBase.Acknowledge(resolvedEvent);
                },
                (eventStorePersistentSubscriptionBase, subscriptionDropReason, exception) => { },
                new UserCredentials("admin", "changeit"));

            
            await db.Connection.AppendToStreamAsync(
                streamName,
                ExpectedVersion.Any,
                new EventData(Guid.NewGuid(), "eventType", true,
                    Encoding.UTF8.GetBytes(expected), null)
            );


            var eventReadResult = await db.Connection.ReadEventAsync(streamName, 0, false);
            var actual = Encoding.UTF8.GetString(eventReadResult.Event.Value.Event.Data);

            Assert.IsNotNull(eventReadResult);
            Assert.That(actual, Is.EqualTo(expected));

            db.Dispose();
        }


        [Test]
        public async Task EmbeddedEventStoreDb_Simple_Append_Event_And_ReadAll_Returns_Event()
        {
            var db = new EmbeddedEventStoreDb.EmbeddedEventStoreDb();

            var expected = new TestEvent() { TestMessage = "TestMessageHere!"};
            await db.AppendAsync("test", -1, new[] { expected });
            var events = await db.ReadAllAsync("test");

            var actual = JsonConvert.DeserializeObject<TestEvent>(Encoding.UTF8.GetString(events.First().Event.Data));
            Assert.That(actual.TestMessage, Is.EqualTo(expected.TestMessage));
        }

        public class TestEvent : IEvent
        {
            public string TestMessage { get; set; }
            public long EventNumber { get; set; }
            public string RaisedBy { get; set; }
            public DateTime RaisedAt { get; set; }
        }
    }
}