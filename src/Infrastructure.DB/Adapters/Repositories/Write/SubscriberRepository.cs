using Application.Ports.Persistence.Write;
using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DB.Adapters.Repositories.Write;

public class SubscriberRepository : ISubscriberRepository
{
    private readonly DatabaseContext _databaseContext;

    public SubscriberRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task<Subscriber?> GetByExternalId(string externalId)
    {
        return await _databaseContext.Subscribers
            .Include(subscriber => subscriber.Subscriptions)
                .ThenInclude(subscription => subscription.Media)
            .SingleOrDefaultAsync(subscriber => subscriber.ExternalIdentifier.Equals(externalId));
    }

    public async Task AddSubscriber(Subscriber subscriber)
    {
        await _databaseContext.SubscriberDbSet.AddAsync(subscriber);
    }
}