using Application.EventHandlers.Base;
using Domain.Model.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DB.DomainEvent;

public class DomainEventPublisher : IDomainEventPublisher
{
    private readonly IServiceProvider serviceProvider;

    public DomainEventPublisher(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public async Task Publish(IDomainEvent domainEvent)
    {
        using var scope = serviceProvider.CreateScope();
        var eventHandlers = scope.ServiceProvider.GetServices<IDomainEventHandler>();

        await Publish(domainEvent, eventHandlers);
    }

    public async Task Publish(IEnumerable<IDomainEvent> domainEvents)
    {
        using var scope = serviceProvider.CreateScope();
        var eventHandlers = scope.ServiceProvider
            .GetServices<IDomainEventHandler>()
            .ToList();

        foreach (var domainEvent in domainEvents)
        {
            await Publish(domainEvent, eventHandlers);
        }
    }

    private static async Task Publish(IDomainEvent domainEvent, IEnumerable<IDomainEventHandler> eventHandlers)
    {
        foreach (var domainEventHandler in eventHandlers)
        {
            await domainEventHandler.Handle(domainEvent);
        }
    }
}