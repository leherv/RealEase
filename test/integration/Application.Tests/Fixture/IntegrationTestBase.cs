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
    private readonly IntegrationTestWebApplicationFactory _webApplicationFactory;

    protected IntegrationTestBase()
    {
        _webApplicationFactory = new IntegrationTestWebApplicationFactory();
        Given = CreateGiven();
        When = CreateWhen();
        Then = CreateThen();
    }
    
    public async Task InitializeAsync()
    {
        await Given.TheDatabase.IsCleared();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    private Given CreateGiven()
    {
        var givenTheData = new GivenTheData();
        var givenTheApplication = new GivenTheApplication(_webApplicationFactory);
        var givenTheDatabase = new GivenTheDatabase(givenTheData, givenTheApplication);
        var givenTheScraper = new GivenTheScraper(_webApplicationFactory.IntegrationTestScraper);

        return new Given(givenTheData, givenTheApplication, givenTheDatabase, givenTheScraper);
    }

    private When CreateWhen()
    {
        var whenTheApplication = new WhenTheApplication(_webApplicationFactory);

        return new When(whenTheApplication);
    }

    private Then CreateThen()
    {
        var thenTheApplication = new ThenTheApplication(_webApplicationFactory);
        var thenTheDatabase = new ThenTheDatabase(thenTheApplication);
        var thenTheNotificationService =
            new ThenTheNotificationService(_webApplicationFactory.IntegrationTestNotificationService);
        var thenTheScraper = new ThenTheScraper(_webApplicationFactory.IntegrationTestScraper);

        return new Then(thenTheDatabase, thenTheApplication, thenTheNotificationService, thenTheScraper);
    }
}