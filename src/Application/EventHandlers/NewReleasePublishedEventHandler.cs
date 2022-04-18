using Application.EventHandlers.Base;
using Application.Ports.Notification;
using Application.Ports.Persistence.Write;

namespace Application.EventHandlers;

public class NewReleasePublishedEventHandler : DomainEventHandler<Domain.Model.Events.NewReleasePublished>
{
    private readonly ISubscriberRepository _subscriberRepository;
    private readonly INotificationService _notificationService;

    public NewReleasePublishedEventHandler(
        INotificationService notificationService,
        ISubscriberRepository subscriberRepository
    )
    {
        _notificationService = notificationService;
        _subscriberRepository = subscriberRepository;
    }

    protected override async Task Execute(Domain.Model.Events.NewReleasePublished newReleasePublishedEvent)
    {
        var (mediaId, mediaName, linkToReleasedResource) = newReleasePublishedEvent;
        
        var subscribers = await _subscriberRepository.GetAllSubscribedToMediaWithId(mediaId);
        foreach (var subscriber in subscribers)
        {
            var releasePublishedNotification = new ReleasePublishedNotification(
                subscriber.ExternalIdentifier,
                mediaName,
                linkToReleasedResource
            );
            await _notificationService.Notify(releasePublishedNotification);
        }
    }
}