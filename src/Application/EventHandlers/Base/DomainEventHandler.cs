using Domain.Model.Base;

namespace Application.EventHandlers.Base;

public abstract class DomainEventHandler<TDomainEvent> : IDomainEventHandler where TDomainEvent : IDomainEvent
{
    public Task Handle(IDomainEvent domainEvent)
    {
        if (domainEvent is TDomainEvent @event)
        {
            return Execute(@event);
        }

        return Task.CompletedTask;
    }

    protected abstract Task Execute(TDomainEvent domainEvent);
}