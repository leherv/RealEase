using Application.UseCases.Base;

namespace Application.UseCases.Subscriber.QueryMediaSubscriptions;

public record MediaSubscriptions : IReadModel
{
    public IReadOnlyCollection<string> SubscribedToMediaNames { get; init; } = new List<string>();

    public MediaSubscriptions(IReadOnlyCollection<string>? subscribedToMediaNames = null)
    {
        SubscribedToMediaNames = subscribedToMediaNames ?? new List<string>();
    }
}