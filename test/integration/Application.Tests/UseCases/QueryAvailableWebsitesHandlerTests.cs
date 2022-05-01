using System.Threading.Tasks;
using Application.Test.Fixture;
using Application.UseCases.Website;
using FluentAssertions;
using Xunit;

namespace Application.Test.UseCases;

public class QueryAvailableWebsitesHandlerTests : IntegrationTestBase
{
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Returns_all_websites()
    {
        await Given.TheDatabase.IsSeeded();
        var availableWebsitesQuery = new AvailableWebsitesQuery();

        var availableWebsites = await When.TheApplication.ReceivesQuery<AvailableWebsitesQuery, AvailableWebsites>(availableWebsitesQuery);

        availableWebsites.Websites
            .Should()
            .HaveCount(Given.The.Website.Websites.Count);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Returns_an_empty_collection_if_no_websites_available()
    {
        var availableWebsitesQuery = new AvailableWebsitesQuery();

        var availableWebsites = await When.TheApplication.ReceivesQuery<AvailableWebsitesQuery, AvailableWebsites>(availableWebsitesQuery);

        availableWebsites.Websites
            .Should()
            .HaveCount(0);
    }
}