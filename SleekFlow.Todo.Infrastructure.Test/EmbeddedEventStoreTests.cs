using System.Text;
using EventStore.ClientAPI;

namespace SleekFlow.Todo.Infrastructure.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task Embedded_EventStore_Connect_Append_Read_Simple_Test()
        {
            var db = new EmbeddedEventStoreDb();
            
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
    }
}