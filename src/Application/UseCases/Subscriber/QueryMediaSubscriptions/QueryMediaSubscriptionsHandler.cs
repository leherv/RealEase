using Application.Ports.Persistence.Read;
using Application.Ports.Persistence.Write;
using Application.UseCases.Base;

namespace Application.UseCases.Subscriber.QueryMediaSubscriptions;

public record MediaSubscriptionsQuery(string ExternalIdentifier);

public sealed class QueryMediaSubscriptionsHandler : IQueryHandler<MediaSubscriptionsQuery, MediaSubscriptions>
{
    private readonly IMediaSubscriptionsReadRepository _mediaSubscriptionsReadRepository;
    private readonly IUnitOfWork _unitOfWork;

    public QueryMediaSubscriptionsHandler(
        IMediaSubscriptionsReadRepository mediaSubscriptionsReadRepository,
        IUnitOfWork unitOfWork
    )
    {
        _mediaSubscriptionsReadRepository = mediaSubscriptionsReadRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<MediaSubscriptions> Handle(MediaSubscriptionsQuery mediaQuery, CancellationToken cancellationToken)
    {
        var subscriber = await _unitOfWork.SubscriberRepository.GetByExternalId(mediaQuery.ExternalIdentifier);
        if (subscriber == null)
            return new MediaSubscriptions();

        return await _mediaSubscriptionsReadRepository.QueryMediaSubscriptionsFor(mediaQuery.ExternalIdentifier);
    }
}