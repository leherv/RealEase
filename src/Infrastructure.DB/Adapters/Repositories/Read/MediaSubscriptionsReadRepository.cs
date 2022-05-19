using Application.Ports.Persistence.Read;
using Application.UseCases.Subscriber.QueryMediaSubscriptions;
using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DB.Adapters.Repositories.Read;

public class MediaSubscriptionsReadRepository : IMediaSubscriptionsReadRepository
{
    private readonly DatabaseContext _databaseContext;

    public MediaSubscriptionsReadRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task<MediaSubscriptions> QueryMediaSubscriptionsFor(string externalIdentifier)
    {
        var subscriber = await GetSubscriber(externalIdentifier);
        if (subscriber == null)
            return new MediaSubscriptions();

        var subscribedToMediaIds = subscriber.SubscribedToMediaIds;

        var subscribedToMediaNames = await GetSubscribedToMediaNames(subscribedToMediaIds);

        return new MediaSubscriptions(subscribedToMediaNames);
    }

    private async Task<List<string>> GetSubscribedToMediaNames(IReadOnlyCollection<Guid> subscribedToMediaIds)
    {
        return await _databaseContext.Media
            .Where(media => subscribedToMediaIds.Contains(media.Id))
            .Select(media => media.Name)
            .ToListAsync();
    }

    private async Task<Subscriber?> GetSubscriber(string externalIdentifier)
    {
        return await _databaseContext.Subscribers
            .Include(subscriber => subscriber.Subscriptions)
            .SingleOrDefaultAsync(subscriber => subscriber.ExternalIdentifier == externalIdentifier);
    }
}