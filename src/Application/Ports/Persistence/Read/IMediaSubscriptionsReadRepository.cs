using Application.UseCases.Subscriber.QueryMediaSubscriptions;

namespace Application.Ports.Persistence.Read;

public interface IMediaSubscriptionsReadRepository
{
    Task<MediaSubscriptions> QueryMediaSubscriptionsFor(string externalIdentifier);
}