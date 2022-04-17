using Domain.Model.Base;

namespace Infrastructure.DB.DomainEvent;

public interface IDomainEventPublisher
{
    Task Publish(IDomainEvent domainEvent);
    Task Publish(IEnumerable<IDomainEvent> domainEvents);
}