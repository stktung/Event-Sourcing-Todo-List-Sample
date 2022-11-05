using System.Text;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using SleekFlow.Todo.Domain.Common;

namespace SleekFlow.Todo.Infrastructure;

public static class EventMapper
{
    public static DomainEvent MapToDomainEvent(ResolvedEvent esEvent)
    {
        var methodInfo = typeof(JsonConvert).GetMethods().FirstOrDefault(
                x => x.Name.Equals("DeserializeObject", StringComparison.OrdinalIgnoreCase) &&
                     x.IsGenericMethod && x.GetParameters().Length == 1);

        var type = Type.GetType($"SleekFlow.Todo.Domain.Event.{esEvent.Event.EventType}, SleekFlow.Todo.Domain");

        if (type == null)
            throw new ArgumentException(
                $"EventType '{esEvent.Event.EventType}' not found. Can not be mapped to domain event.");

        var genericMethod = methodInfo!.MakeGenericMethod(type);
        var data = Encoding.UTF8.GetString(esEvent.Event.Data);
        var domainEvent = (DomainEvent?)genericMethod.Invoke(null, new[] { data });

        if (domainEvent == null)
            throw new ArgumentException($"Unable to deserialize event. Type: '{esEvent.Event.EventType}' Data: '{data}'");

        domainEvent.EventNumber = esEvent.Event.EventNumber;
        domainEvent.RaisedAt = DateTimeOffset.FromUnixTimeMilliseconds(esEvent.Event.CreatedEpoch).UtcDateTime;
        
        return domainEvent!;
    }
}