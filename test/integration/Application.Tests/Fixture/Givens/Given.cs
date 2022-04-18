
namespace Application.Test.Fixture.Givens;

public class Given
{
    public GivenTheData A { get; }
    public GivenTheData An { get; }
    public GivenTheData The { get; }
    public GivenTheDatabase TheDatabase { get; }
    public GivenTheApplication TheApplication { get; }
    public GivenTheScraper TheScraper { get; }
    
    public Given(
        GivenTheData givenTheData,
        GivenTheApplication givenTheApplication,
        GivenTheDatabase givenTheDatabase,
        GivenTheScraper theScraper
    )
    {
        A = givenTheData;
        An = givenTheData;
        The = givenTheData;

        TheApplication = givenTheApplication;
        TheDatabase = givenTheDatabase;
        TheScraper = theScraper;
    }
}