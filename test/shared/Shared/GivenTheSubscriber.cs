using Domain.Model;
using Domain.Results;

namespace Shared;

public class GivenTheSubscriber
{
    public static Guid SubscriberId1 = Guid.Parse("B95AD548-0BE7-4222-A0FC-FC8E77662E6F");
    public static Guid SubscriberId2 = Guid.Parse("E95AD548-0BE7-4222-A0FC-FC8E77662E6F");
    public static readonly string SubscriberExternalId1 = Guid.Parse("F95AD548-0BE7-4323-A0FC-FC8E77662E6E").ToString();
    public static readonly string SubscriberExternalId2 = Guid.Parse("A95AD548-0BE7-4223-A0FC-FC8E77662E6E").ToString();
    
    public List<Subscriber> Subscribers { get; }
    
    public Subscriber WithSubscriptions { get; }
    
    public IReadOnlyCollection<Media> SubscribedToMedia { get; }
    
    public IReadOnlyCollection<Media> NotSubscribedToMedia { get; }

    public Subscriber WithoutSubscriptions { get; }
    
    public GivenTheSubscriber(GivenTheMedia givenTheMedia)
    {
        SubscribedToMedia = givenTheMedia.MediaList.Take(2).ToList();
        NotSubscribedToMedia = givenTheMedia.MediaList.Skip(2).ToList();
        WithSubscriptions = Create(SubscriberId1, SubscriberExternalId1).Value;
        foreach (var media in SubscribedToMedia)
        {
            WithSubscriptions.Subscribe(media.Id);
        }

        WithoutSubscriptions = Create(SubscriberId2, SubscriberExternalId2).Value;
        
        Subscribers = new List<Subscriber>
        {
            WithSubscriptions,
            WithoutSubscriptions
        };
    }

    public static Result<Subscriber> Create(
        Guid? id = null,
        string? externalIdentifier = null
    )
    {
        return Subscriber.Create(
            id ?? Guid.NewGuid(),
            externalIdentifier ?? Guid.NewGuid().ToString()
        );
    }
}