using Domain.Model.Base;

namespace Application.UseCases.Base.DomainEvent;

public interface IDomainEventHandler
{
    Task Handle(IDomainEvent domainEvent);
}