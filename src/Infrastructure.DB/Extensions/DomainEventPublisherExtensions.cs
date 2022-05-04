using Domain.Model.Base;
using Infrastructure.DB.DomainEvent;

namespace Infrastructure.DB.Extensions;

public static class DomainEventPublisherExtensions
{
    public static async Task PublishDomainEvents(this IDomainEventPublisher publisher, DatabaseContext ctx)
    {
        var domainEntities = ctx.ChangeTracker
            .Entries<AggregateRoot>()
            .Where(aggregateRoot => aggregateRoot.Entity.DomainEvents.Any())
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

        await publisher.Publish(domainEvents);
    }
}