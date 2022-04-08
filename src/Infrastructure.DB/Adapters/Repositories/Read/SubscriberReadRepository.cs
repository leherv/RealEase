using Application.Ports.Persistence.Read;
using Application.UseCases.Subscriber.QueryMediaSubscriptions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DB.Adapters.Repositories.Read;

public class SubscriberReadRepository : ISubscriberReadRepository
{
    private readonly DatabaseContext _databaseContext;

    public SubscriberReadRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task<MediaSubscriptions> QueryMediaSubscriptionsFor(string externalIdentifier)
    {
        var subscriber = await _databaseContext.Subscribers
            .Include(subscriber => subscriber.Subscriptions)
                .ThenInclude(subscription => subscription.Media)
            .SingleOrDefaultAsync(subscriber => subscriber.ExternalIdentifier == externalIdentifier);

        return subscriber == null
            ? new MediaSubscriptions()
            : new MediaSubscriptions(subscriber.SubscribedToMedia.Select(media => media.Name).ToList());
    }
}