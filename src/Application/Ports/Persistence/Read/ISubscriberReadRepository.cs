using Application.UseCases.Subscriber.QueryMediaSubscriptions;

namespace Application.Ports.Persistence.Read;

public interface ISubscriberReadRepository
{
    Task<MediaSubscriptions> QueryMediaSubscriptionsFor(string externalIdentifier);
}