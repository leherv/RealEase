using Domain.Results;

namespace Application.Test.Fixture.Thens;

public class Then
{
    public ThenTheDatabase TheDatabase { get; }
    public ThenTheApplication TheApplication { get; }
    public ThenTheNotificationService TheNotificationService { get; }
    public ThenTheScraper TheScraper { get; }
    public ThenTheMediaNameScraper TheMediaNameScraper { get; }
    
    public Then(
        ThenTheDatabase theDatabase,
        ThenTheApplication theApplication,
        ThenTheNotificationService theNotificationService,
        ThenTheScraper theScraper,
        ThenTheMediaNameScraper theMediaNameScraper
    )
    {
        TheDatabase = theDatabase;
        TheApplication = theApplication;
        TheNotificationService = theNotificationService;
        TheScraper = theScraper;
        TheMediaNameScraper = theMediaNameScraper;
    }
    
    public ThenTheResult TheResult(Result response)
    {
        return new ThenTheResult(response);
    }
}