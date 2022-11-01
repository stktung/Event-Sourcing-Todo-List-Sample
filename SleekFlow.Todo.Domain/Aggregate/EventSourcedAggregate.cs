using System.Collections.ObjectModel;

namespace SleekFlow.Todo.Domain.Aggregate;

public abstract class EventSourcedAggregate
{
    private readonly List<IEvent> _pastEvents = new();
    private readonly List<IEvent> _newEvents = new();

    protected void Raise(IEvent e)
    {
        _newEvents.Add(e);
    }

    public int PreviousRevision => _pastEvents.Count() - 1;

    public ReadOnlyCollection<IEvent> NewEvents => _newEvents.AsReadOnly();
}