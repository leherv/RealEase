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

    private Given CreateGiven()
    {
        var givenTheData = new GivenTheData();
        var givenTheApplication = new GivenTheApplication(_webApplicationFactory);
        var givenTheDatabase = new GivenTheDatabase(givenTheData, givenTheApplication);

        return new Given(givenTheData, givenTheApplication, givenTheDatabase);
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

        return new Then(thenTheDatabase, thenTheApplication);
    }
    
    public async Task DisposeAsync()
    {
        await _webApplicationFactory.DisposeAsync();
    }
}