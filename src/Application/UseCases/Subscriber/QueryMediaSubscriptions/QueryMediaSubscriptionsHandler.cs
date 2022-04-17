using Application.Ports.Persistence.Read;
using Application.Ports.Persistence.Write;
using Application.UseCases.Base;
using Application.UseCases.Base.CQS;

namespace Application.UseCases.Subscriber.QueryMediaSubscriptions;

public record MediaSubscriptionsQuery(string ExternalIdentifier);

public sealed class QueryMediaSubscriptionsHandler : IQueryHandler<MediaSubscriptionsQuery, MediaSubscriptions>
{
    private readonly ISubscriberReadRepository _subscriberReadRepository;
    private readonly IUnitOfWork _unitOfWork;

    public QueryMediaSubscriptionsHandler(
        ISubscriberReadRepository subscriberReadRepository,
        IUnitOfWork unitOfWork
    )
    {
        _subscriberReadRepository = subscriberReadRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<MediaSubscriptions> Handle(MediaSubscriptionsQuery mediaSubscriptionsQuery, CancellationToken cancellationToken)
    {
        var subscriber = await _unitOfWork.SubscriberRepository.GetByExternalId(mediaSubscriptionsQuery.ExternalIdentifier);
        if (subscriber == null)
            return new MediaSubscriptions();

        return await _subscriberReadRepository.QueryMediaSubscriptionsFor(mediaSubscriptionsQuery.ExternalIdentifier);
    }
}