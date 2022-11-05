using System.Text;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using SleekFlow.Todo.Domain;
using SleekFlow.Todo.Domain.Common;

namespace SleekFlow.Todo.Infrastructure;

public static class EventMapper
{
    public static IEvent MapToDomainEvent(ResolvedEvent esEvent)
    {
        switch (esEvent.Event.EventType)
        {
            case nameof(TodoCreatedEvent):
                var e =
                    JsonConvert.DeserializeObject<TodoCreatedEvent>(
                        Encoding.UTF8.GetString(esEvent.Event.Data));

                e.EventNumber = esEvent.Event.EventNumber;
                e.RaisedAt = DateTimeOffset.FromUnixTimeMilliseconds(esEvent.Event.CreatedEpoch).UtcDateTime;

                return e;
            default:
                throw new ArgumentException(
                    $"EventType '{esEvent.Event.EventType}' not found. Can not be mapped to domain event.");
        }
    }
}