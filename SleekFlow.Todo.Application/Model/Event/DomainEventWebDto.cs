using JsonSubTypes;
using Newtonsoft.Json;
using SleekFlow.Todo.Domain.Event;

namespace SleekFlow.Todo.Application.Model.Event;

[JsonConverter(typeof(JsonSubtypes), "EventType")]
[JsonSubtypes.KnownSubType(typeof(TodoCreatedEventWebDto), nameof(TodoCreatedEvent))]
[JsonSubtypes.KnownSubType(typeof(TodoNameTextInsertedEventWebDto), nameof(TodoNameTextInsertedEvent))]
[JsonSubtypes.KnownSubType(typeof(TodoNameTextDeletedEventWebDto), nameof(TodoNameTextDeletedEvent))]
[JsonSubtypes.KnownSubType(typeof(TodoDescriptionTextInsertedEventWebDto), nameof(TodoDescriptionTextInsertedEvent))]
[JsonSubtypes.KnownSubType(typeof(TodoDescriptionTextDeletedEventWebDto), nameof(TodoDescriptionTextDeletedEvent))]
[JsonSubtypes.KnownSubType(typeof(TodoDueDateUpdatedEventWebDto), nameof(TodoDueDateUpdatedEvent))]
[JsonSubtypes.KnownSubType(typeof(TodoCompleteMarkedEventWebDto), nameof(TodoCompleteMarkedEvent))]
[JsonSubtypes.KnownSubType(typeof(TodoCompleteUnmarkedEventWebDto), nameof(TodoCompleteUnmarkedEvent))]
public class DomainEventWebDto
{
    public string EventType { get; set; }
    public Guid Id { get; set; }
    public long EventNumber { get; set; }
    public string RaisedBy { get; set; }
    public DateTime RaisedAt { get; set; }
}