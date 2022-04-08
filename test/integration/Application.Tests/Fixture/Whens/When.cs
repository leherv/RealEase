using System;

namespace Application.Test.Fixture.Whens;

public class When : IDisposable
{
    public WhenTheApplication TheApplication { get; }
    
    public When(WhenTheApplication theApplication)
    {
        TheApplication = theApplication;
    }

    public void Dispose()
    {
        TheApplication.Dispose();
    }
}