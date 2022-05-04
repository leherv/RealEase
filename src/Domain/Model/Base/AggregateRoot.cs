namespace Domain.Model.Base;

public abstract class AggregateRoot : Entity
{
    public IEnumerable<IDomainEvent> DomainEvents => _domainEvents;
    private readonly List<IDomainEvent> _domainEvents = new();
    
    protected AggregateRoot(Guid id) : base(id)
    {
    }
    
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}