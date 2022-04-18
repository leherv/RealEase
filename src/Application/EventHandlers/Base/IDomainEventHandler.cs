using Domain.Model.Base;

namespace Application.EventHandlers.Base;

public interface IDomainEventHandler
{
    Task Handle(IDomainEvent domainEvent);
}