using Application.UseCases.Base;

namespace Application.UseCases.Subscriber.QueryMediaSubscriptions;

public record MediaSubscriptions : IReadModel
{
    public IReadOnlyCollection<MediaSubscriptionInfo> SubscribedToMedia { get; init; } = new List<MediaSubscriptionInfo>();

    public MediaSubscriptions(IReadOnlyCollection<MediaSubscriptionInfo>? subscribedToMedia = null)
    {
        SubscribedToMedia = subscribedToMedia ?? new List<MediaSubscriptionInfo>();
    }
}