
namespace Application.Test.Fixture.Givens;

public class Given
{
    public GivenTheData A { get; }
    public GivenTheData An { get; }
    public GivenTheData The { get; }
    public GivenTheDatabase TheDatabase { get; }
    public GivenTheApplication TheApplication { get; }
    
    public Given(
        GivenTheData givenTheData,
        GivenTheApplication givenTheApplication,
        GivenTheDatabase givenTheDatabase
    )
    {
        A = givenTheData;
        An = givenTheData;
        The = givenTheData;

        TheApplication = givenTheApplication;
        TheDatabase = givenTheDatabase;
    }
}