using System.Threading.Tasks;
using Application.Test.Fixture.Givens;
using Application.Test.Fixture.Thens;
using Application.Test.Fixture.Whens;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace Application.Test.Fixture;

public abstract class IntegrationTestBase : IAsyncLifetime
{
    protected readonly Given Given;
    protected readonly When When;
    protected readonly Then Then;

    protected IntegrationTestBase()
    {
        var webApplicationFactory = new IntegrationTestWebApplicationFactory();
        Given = CreateGiven(webApplicationFactory);
        When = CreateWhen(webApplicationFactory);
        Then = CreateThen(webApplicationFactory);
    }
    
    public async Task InitializeAsync()
    {
        await Given.TheDatabase.IsCleared();
    }

    private static Given CreateGiven(IntegrationTestWebApplicationFactory webApplicationFactory)
    {
        var givenTheData = new GivenTheData();
        var givenTheApplication = new GivenTheApplication(webApplicationFactory);
        var givenTheDatabase = new GivenTheDatabase(givenTheData, givenTheApplication);

        return new Given(givenTheData, givenTheApplication, givenTheDatabase);
    }

    private static When CreateWhen(IntegrationTestWebApplicationFactory webApplicationFactory)
    {
        var whenTheApplication = new WhenTheApplication(webApplicationFactory);

        return new When(whenTheApplication);
    }

    private static Then CreateThen(IntegrationTestWebApplicationFactory webApplicationFactory)
    {
        var thenTheApplication = new ThenTheApplication(webApplicationFactory);
        var thenTheDatabase = new ThenTheDatabase(thenTheApplication);

        return new Then(thenTheDatabase, thenTheApplication);
    }
    
    public async Task DisposeAsync()
    {
        await Given.TheDatabase.IsCleared();
        Given.Dispose();
        When.Dispose();
        Then.Dispose();
    }
}