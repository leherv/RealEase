
namespace Application.Test.Fixture.Whens;

public class When
{
    public WhenTheApplication TheApplication { get; }
    
    public When(WhenTheApplication theApplication)
    {
        TheApplication = theApplication;
    }
}