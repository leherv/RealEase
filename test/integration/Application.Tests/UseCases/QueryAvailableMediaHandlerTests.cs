using System;
using System.Linq;
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

        var mediaCount = Given.The.Media.PersistedMediaList.Count;
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

        var mediaCount = Given.The.Media.PersistedMediaList.Count;
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
    
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Returns_only_media_with_mediaName_if_complete_mediaName_is_searched_for()
    {
        await Given.TheDatabase.IsSeeded();
        var mediaNameSearchString = Given.A.Media.WithSubscriberWithReleases.Name;
        var numberOfMediaContainingSearchString = MediaCountContaining(mediaNameSearchString);
        var availableMediaQuery = new AvailableMediaQuery(1, 50, mediaNameSearchString);

        var availableMedia = await When.TheApplication.ReceivesQuery<AvailableMediaQuery, AvailableMedia>(availableMediaQuery);;

        availableMedia.Media
            .Should()
            .HaveCount(numberOfMediaContainingSearchString);
        availableMedia.TotalResultCount.Should().Be(numberOfMediaContainingSearchString);
        availableMedia.Media.First().Name.Should().Be(mediaNameSearchString);
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Returns_only_media_with_mediaName_containing_mediaNameSearchString()
    {
        await Given.TheDatabase.IsSeeded();
        const string mediaNameSearchString = "n";
        var numberOfMediaContainingSearchString = MediaCountContaining(mediaNameSearchString);
        var availableMediaQuery = new AvailableMediaQuery(1, 50, mediaNameSearchString);

        var availableMedia = await When.TheApplication.ReceivesQuery<AvailableMediaQuery, AvailableMedia>(availableMediaQuery);;

        availableMedia.Media
            .Should()
            .HaveCount(numberOfMediaContainingSearchString);
        availableMedia.TotalResultCount.Should().Be(numberOfMediaContainingSearchString);
    }
    
    [Theory]
    [Trait("Category", "Integration")]
    [InlineData("")]
    [InlineData(null)]
    public async Task Should_return_all_media_if_queryString_is_not_specified(string? mediaNameSearchString)
    {
        await Given.TheDatabase.IsSeeded();
        var availableMediaQuery = new AvailableMediaQuery(1, 50, mediaNameSearchString);

        var availableMedia = await When.TheApplication.ReceivesQuery<AvailableMediaQuery, AvailableMedia>(availableMediaQuery);

        var mediaCount = Given.The.Media.PersistedMediaList.Count;
        availableMedia.Media
            .Should()
            .HaveCount(mediaCount > 50 ? 50 : mediaCount);
        availableMedia.TotalResultCount.Should().Be(mediaCount);
    }

    private int MediaCountContaining(string mediaNameSearchString)
    {
        return Given.The.Media.PersistedMediaList
            .Count(media => media.Name.Contains(mediaNameSearchString, StringComparison.InvariantCultureIgnoreCase));
    }
}