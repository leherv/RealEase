using System;
using System.Threading.Tasks;
using Application.Test.Fixture;
using Application.UseCases.Media.QueryMedia;
using Domain.ApplicationErrors;
using Domain.Results;
using FluentAssertions;
using Xunit;

namespace Application.Test.UseCases;

public class QueryMediaHandlerTests : IntegrationTestBase
{
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Returns_media_if_it_exists()
    {
        await Given.TheDatabase.IsSeeded();
        var mediaToQuery = Given.A.Media.WithSubscriberWithReleases;
        var mediaQuery = new MediaQuery(mediaToQuery.Id);

        var mediaResult = await When.TheApplication.ReceivesQuery<MediaQuery, Result<MediaDetails>>(mediaQuery);

        Then.TheResult(mediaResult)
            .IsSuccessful()
            .Should()
            .BeTrue();
        
        var mediaInfo = mediaResult.Value;
        mediaInfo.Id.Should().Be(mediaToQuery.Id);
        mediaInfo.Name.Should().Be(mediaToQuery.Name);
        mediaInfo.ScrapeTargetDetails.Should().HaveCount(mediaToQuery.ScrapeTargets.Count);
        mediaInfo.ReleaseDetails.LatestReleaseMajor.Should().Be(mediaToQuery.NewestRelease.ReleaseNumber.Major);
        mediaInfo.ReleaseDetails.LatestReleaseMinor.Should().Be(mediaToQuery.NewestRelease.ReleaseNumber.Minor);
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public async Task ReleaseDetails_are_not_present_if_no_release_yet()
    {
        await Given.TheDatabase.IsSeeded();
        var mediaToQuery = Given.A.Media.WithSubscriberWithTwoScrapeTargets;
        var mediaQuery = new MediaQuery(mediaToQuery.Id);

        var mediaResult = await When.TheApplication.ReceivesQuery<MediaQuery, Result<MediaDetails>>(mediaQuery);

        Then.TheResult(mediaResult)
            .IsSuccessful()
            .Should()
            .BeTrue();
        
        var mediaInfo = mediaResult.Value;
        mediaInfo.Id.Should().Be(mediaToQuery.Id);
        mediaInfo.Name.Should().Be(mediaToQuery.Name);
        mediaInfo.ScrapeTargetDetails.Should().HaveCount(mediaToQuery.ScrapeTargets.Count);
        mediaInfo.ReleaseDetails.Should().Be(null);
        mediaInfo.ReleaseDetails.Should().Be(null);
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Returns_failure_if_no_media_with_id_exists()
    {
        await Given.TheDatabase.IsSeeded();
        var mediaQuery = new MediaQuery(Guid.NewGuid());

        var mediaResult = await When.TheApplication.ReceivesQuery<MediaQuery, Result<MediaDetails>>(mediaQuery);

        Then.TheResult(mediaResult)
            .IsAFailure()
            .Should()
            .BeTrue();

        Then.TheResult(mediaResult)
            .ContainsErrorWithCode(Errors.General.NotFoundErrorCode)
            .Should()
            .BeTrue();
    }
}