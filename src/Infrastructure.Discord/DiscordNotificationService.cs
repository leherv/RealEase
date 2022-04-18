using Application.Ports.Notification;

namespace Infrastructure.Discord;

public class DiscordNotificationService : INotificationService
{
    public Task Notify(ReleasePublishedNotification releasePublishedNotification)
    {
        return Task.CompletedTask;
    }
}