using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Ports.Scraper;
using Application.Test.Fixture;
using Application.UseCases.Scrape;
using Domain.Model;
using Domain.Results;
using FluentAssertions;
using Xunit;

namespace Application.Test.UseCases;

public class ScrapeNewReleasesHandlerTests : IntegrationTestBase
{
    [Fact]
    public async Task Publishes_newer_release_if_found()
    {
        await Given.TheDatabase.IsSeeded();
        var mediaName = Given.A.Media.WithoutReleases.Name;
        var scrapeResult = new ScrapedMediaRelease(
            mediaName,
            "https://www.test.com/chapter/1",
            1
        );
        Given.TheScraper.ScrapeForMediaWithNameReturns(mediaName, Result<ScrapedMediaRelease>.Success(scrapeResult));
        var scrapeNewReleasesCommand = new ScrapeNewReleasesCommand(new List<string> {mediaName});

        var scrapeNewReleasesResult = await When.TheApplication.ReceivesCommand<ScrapeNewReleasesCommand, Result>(scrapeNewReleasesCommand);

        Then.TheResult(scrapeNewReleasesResult).IsSuccessful();
        var media = await Then.TheDatabase.Query(unitOfWork => unitOfWork.MediaRepository.GetByName(mediaName));
        media.Should().NotBeNull();
        media!.NewestRelease.Should().NotBeNull();
        media.NewestRelease!.ReleaseNumber.Should().Be(ReleaseNumber.Create(1, 0).Value);
    }
    
    [Fact]
    public async Task Does_not_call_notificationService_if_no_subscribers()
    {
        await Given.TheDatabase.IsSeeded();
        var mediaName = Given.A.Media.WithoutSubscribersAndReleases.Name;
        var scrapeResult = new ScrapedMediaRelease(
            mediaName,
            "https://www.test.com/chapter/1",
            1
        );
        Given.TheScraper.ScrapeForMediaWithNameReturns(mediaName, Result<ScrapedMediaRelease>.Success(scrapeResult));
        var scrapeNewReleasesCommand = new ScrapeNewReleasesCommand(new List<string> {mediaName});

        var scrapeNewReleasesResult = await When.TheApplication.ReceivesCommand<ScrapeNewReleasesCommand, Result>(scrapeNewReleasesCommand);
        
        Then.TheResult(scrapeNewReleasesResult).IsSuccessful();
        Then.TheNotificationService.HasNotBeenCalled();
    }
}