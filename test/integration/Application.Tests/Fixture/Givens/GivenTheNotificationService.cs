using Application.Ports.Notification;
using Domain.Results;
using FakeItEasy;

namespace Application.Test.Fixture.Givens;

public class GivenTheNotificationService
{
    private readonly INotificationService _notificationService;

    public GivenTheNotificationService(INotificationService notificationService)
    {
        _notificationService = notificationService;
        NotifyReturns(Result.Success());
    }

    public void NotifyReturns(Result result)
    {
        A.CallTo(() => _notificationService.Notify(A<ReleasePublishedNotification>._)).Returns(result);
    }
}