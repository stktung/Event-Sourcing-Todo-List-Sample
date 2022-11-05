using System.Collections.ObjectModel;

namespace SleekFlow.Todo.Domain.Common;

public abstract class EventSourcedAggregate
{
    protected readonly List<DomainEvent> _pastEvents = new();
    protected readonly List<DomainEvent> _newEvents = new();
    protected readonly string _streamPrefix;

    protected EventSourcedAggregate(string streamPrefix)
    {
        _streamPrefix = streamPrefix;
    }

    public Guid Id { get; protected set; }

    protected void Raise(DomainEvent e)
    {
        _newEvents.Add(e);
    }

    public int PreviousRevision => _pastEvents.Count() - 1;

    public ReadOnlyCollection<DomainEvent> NewEvents => _newEvents.AsReadOnly();

    
}