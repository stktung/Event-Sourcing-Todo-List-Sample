using System.Collections.ObjectModel;

namespace SleekFlow.Todo.Domain.Common;

public abstract class EventSourcedAggregate
{
    protected readonly List<IEvent> _pastEvents = new();
    protected readonly List<IEvent> _newEvents = new();
    protected readonly string _streamPrefix;

    protected EventSourcedAggregate(string streamPrefix)
    {
        _streamPrefix = streamPrefix;
    }

    public Guid Id { get; protected set; }

    protected void Raise(IEvent e)
    {
        _newEvents.Add(e);
    }

    public int PreviousRevision => _pastEvents.Count() - 1;

    public ReadOnlyCollection<IEvent> NewEvents => _newEvents.AsReadOnly();

    
}