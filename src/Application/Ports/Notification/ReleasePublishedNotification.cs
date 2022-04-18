namespace Application.Ports.Notification;

public record ReleasePublishedNotification(string SubscriberExternalIdentifier, string MediaName, string LinkToReleasedResource);