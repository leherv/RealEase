using Domain.Model.Base;

namespace Domain.Model;

public class Subscription : Entity
{
    public Guid SubscriberId { get; }
    public Guid MediaId { get; }

    private Subscription(Guid id, Guid mediaId, Guid subscriberId) : base(id)
    {
        MediaId = mediaId;
        SubscriberId = subscriberId;
    }

    public static Subscription Create(Guid id, Guid mediaId, Guid subscriberId)
    {
        return new Subscription(id, mediaId, subscriberId);
    }
}