using Application.Test.Fixture.Shared;

namespace Application.Test.Fixture.Givens;

public class GivenTheApplication : TheApplication
{
    public GivenTheApplication(IntegrationTestWebApplicationFactory factory) : base(factory)
    {
    }
}