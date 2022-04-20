using Application.Ports.Notification;
using FakeItEasy;

namespace Application.Test.Fixture.Thens;

public class ThenTheNotificationService
{
    private readonly INotificationService _notificationService;

    public ThenTheNotificationService(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public void HasBeenCalledWithReleasePublishedNotificationOnce(
        ReleasePublishedNotification releasePublishedNotification)
    {
        A.CallTo(() =>
            _notificationService.Notify(A<ReleasePublishedNotification>.That.Matches(notification =>
                notification.Equals(releasePublishedNotification))))
            .MustHaveHappenedOnceExactly();
    }

    public void HasNotBeenCalled()
    {
        A.CallTo(() => _notificationService.Notify(A<ReleasePublishedNotification>._)).MustNotHaveHappened();
    }
}