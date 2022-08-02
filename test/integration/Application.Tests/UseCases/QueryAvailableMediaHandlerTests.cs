using System.Threading.Tasks;
using Application.Test.Fixture;
using Application.UseCases.Media.QueryAvailableMedia;
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
        var availableMediaQuery = new AvailableMediaQuery(1, 50);

        var availableMedia = await When.TheApplication.ReceivesQuery<AvailableMediaQuery, AvailableMedia>(availableMediaQuery);

        var mediaCount = Given.The.Media.MediaList.Count;
        availableMedia.Media
            .Should()
            .HaveCount(mediaCount > 50 ? 50 : mediaCount);
        availableMedia.TotalResultCount.Should().Be(mediaCount);
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Returns_PageSize_Media_at_most()
    {
        await Given.TheDatabase.IsSeeded();
        const int pageSize = 1;
        var availableMediaQuery = new AvailableMediaQuery(1, pageSize);

        var availableMedia = await When.TheApplication.ReceivesQuery<AvailableMediaQuery, AvailableMedia>(availableMediaQuery);

        var mediaCount = Given.The.Media.MediaList.Count;
        availableMedia.Media
            .Should()
            .HaveCount(pageSize);
        availableMedia.TotalResultCount.Should().Be(mediaCount);
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Returns_an_empty_list_if_no_media_available()
    {
        var availableMediaQuery = new AvailableMediaQuery(1, 50);

        var availableMedia = await When.TheApplication.ReceivesQuery<AvailableMediaQuery, AvailableMedia>(availableMediaQuery);;

        availableMedia.Media
            .Should()
            .HaveCount(0);
        availableMedia.TotalResultCount.Should().Be(0);
    }
}