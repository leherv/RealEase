using Domain.Model;

namespace Application.Ports.Persistence.Write;

public interface ISubscriberRepository
{
    Task<Subscriber?> GetByExternalId(string externalId);

    Task AddSubscriber(Subscriber subscriber);

    Task<IReadOnlyCollection<Subscriber>> GetAllSubscribedToMediaWithId(Guid mediaId);
}