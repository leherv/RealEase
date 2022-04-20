using Application.EventHandlers.Base;
using Application.Ports.General;
using Application.Ports.Notification;
using Application.Ports.Persistence.Write;

namespace Application.EventHandlers;

public class NewReleasePublishedEventHandler : DomainEventHandler<Domain.Model.Events.NewReleasePublished>
{
    private readonly ISubscriberRepository _subscriberRepository;
    private readonly INotificationService _notificationService;
    private readonly IApplicationLogger _applicationLogger;

    public NewReleasePublishedEventHandler(
        INotificationService notificationService,
        ISubscriberRepository subscriberRepository,
        IApplicationLogger applicationLogger
    )
    {
        _notificationService = notificationService;
        _subscriberRepository = subscriberRepository;
        _applicationLogger = applicationLogger;
    }

    protected override async Task Execute(Domain.Model.Events.NewReleasePublished newReleasePublishedEvent)
    {
        var (mediaId, mediaName, linkToReleasedResource) = newReleasePublishedEvent;
        
        var subscribers = await _subscriberRepository.GetAllSubscribersByMediaId(mediaId);
        foreach (var subscriber in subscribers)
        {
            var releasePublishedNotification = new ReleasePublishedNotification(
                subscriber.ExternalIdentifier,
                mediaName,
                linkToReleasedResource
            );
            var notificationResult = await _notificationService.Notify(releasePublishedNotification);
            if(notificationResult.IsFailure)
                _applicationLogger.LogWarning(
                    $"Notifying subscriber with externalIdentifier {subscriber.ExternalIdentifier} failed due to {notificationResult.Error}"
                );
        }
    }
}