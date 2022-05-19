using Domain.ApplicationErrors;
using Domain.Invariants;
using Domain.Invariants.Extensions;
using Domain.Model.Base;
using Domain.Results;

namespace Domain.Model;

public class Subscriber : AggregateRoot
{
    public string ExternalIdentifier { get; }

    private List<Subscription> _subscriptions;
    public IReadOnlyCollection<Subscription> Subscriptions => _subscriptions;
    public IReadOnlyCollection<Guid> SubscribedToMediaIds => _subscriptions
        .Select(subscription => subscription.MediaId)
        .ToList();

    private Subscriber(Guid id, string externalIdentifier) : base(id)
    {
        ExternalIdentifier = externalIdentifier;
        _subscriptions = new List<Subscription>();
    }

    public static Result<Subscriber> Create(Guid id, string externalIdentifier)
    {
        return Invariant.Create
            .NotNullOrWhiteSpace(externalIdentifier, nameof(externalIdentifier))
            .ValidateAndCreate(() => new Subscriber(id, externalIdentifier));
    }

    public void Subscribe(Guid mediaId)
    {
        var subscription = Subscription.Create(Guid.NewGuid(), mediaId, Id);
        if (!SubscribedToMediaIds.Contains(mediaId))
            _subscriptions.Add(subscription);
    }

    public Result Unsubscribe(Guid mediaId)
    {
        var subscription = _subscriptions.SingleOrDefault(subscription => subscription.MediaId.Equals(mediaId));
        return Result.SuccessIf(
            subscription == null || _subscriptions.Remove(subscription),
            Errors.Subscriber.UnsubscribeFailedError(mediaId)
        );
    }
}