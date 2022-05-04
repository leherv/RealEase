using Domain.Results;

namespace Application.Ports.Notification;

public interface INotificationService
{
    Task<Result> Notify(ReleasePublishedNotification releasePublishedNotification);
}