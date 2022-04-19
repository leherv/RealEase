using Application.Ports.Notification;
using FakeItEasy;

namespace Application.Test.Fixture.Thens;

public class ThenTheNotificationService
{
    public INotificationService NotificationService;

    public ThenTheNotificationService(INotificationService notificationService)
    {
        NotificationService = notificationService;
    }

    public void HasBeenCalledWithReleasePublishedNotificationOnce(
        ReleasePublishedNotification releasePublishedNotification)
    {
        A.CallTo(() =>
            NotificationService.Notify(A<ReleasePublishedNotification>.That.Matches(notification =>
                notification.Equals(releasePublishedNotification))))
            .MustHaveHappenedOnceExactly();
    }

    public void HasNotBeenCalled()
    {
        A.CallTo(() => NotificationService.Notify(A<ReleasePublishedNotification>._)).MustNotHaveHappened();
    }
}