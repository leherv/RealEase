using Domain.Results;

namespace Application.Test.Fixture.Thens;

public class Then
{
    public ThenTheDatabase TheDatabase { get; }
    public ThenTheApplication TheApplication { get; }
    public ThenTheNotificationService TheNotificationService { get; }
    
    public Then(
        ThenTheDatabase theDatabase,
        ThenTheApplication theApplication,
        ThenTheNotificationService theNotificationService
    )
    {
        TheDatabase = theDatabase;
        TheApplication = theApplication;
        TheNotificationService = theNotificationService;
    }
    
    public ThenTheResult TheResult(Result response)
    {
        return new ThenTheResult(response);
    }
}