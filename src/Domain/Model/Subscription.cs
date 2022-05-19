using Domain.Model.Base;

namespace Domain.Model;

public class Subscription : Entity
{
    // TODO: make only id so other aggregate is not directly contained but only referenced
    public Media Media { get; }
    public Guid SubscriberId { get; }
    public Guid MediaId { get; }

    private Subscription(Guid id, Media media, Guid subscriberId) : base(id)
    {
        Media = media;
        MediaId = media.Id;
        SubscriberId = subscriberId;
    }

    // only for ef core
    private Subscription(Guid id): base(id)
    {
        
    }

    public static Subscription Create(Guid id, Media media, Guid subscriberId)
    {
        return new Subscription(id, media, subscriberId);
    }
}