namespace Application.Ports.Notification;

public interface INotificationService
{
    Task Notify(ReleasePublishedNotification releasePublishedNotification);
}