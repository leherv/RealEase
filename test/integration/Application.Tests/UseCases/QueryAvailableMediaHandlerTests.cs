using System.Threading.Tasks;
using Application.Test.Fixture;
using Application.UseCases.Media;
using FluentAssertions;
using Xunit;

namespace Application.Test.UseCases;

public class QueryAvailableMediaHandlerTests : IntegrationTestBase
{
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Returns_all_media()
    {
        await Given.TheDatabase.IsSeeded();
        var availableMediaQuery = new AvailableMediaQuery();

        var availableMedia = await When.TheApplication.ReceivesQuery<AvailableMediaQuery, AvailableMedia>(availableMediaQuery);

        availableMedia.MediaNames
            .Should()
            .HaveCount(Given.The.Media.MediaList.Count);
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Returns_an_empty_list_if_no_media_available()
    {
        var availableMediaQuery = new AvailableMediaQuery();

        var availableMedia = await When.TheApplication.ReceivesQuery<AvailableMediaQuery, AvailableMedia>(availableMediaQuery);;

        availableMedia.MediaNames
            .Should()
            .HaveCount(0);
    }
}