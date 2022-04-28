using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Ports.Notification;
using Application.Ports.Scraper;
using Application.Test.Fixture;
using Application.UseCases.Scrape;
using Domain.ApplicationErrors;
using Domain.Model;
using Domain.Results;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Application.Test.UseCases;

public class ScrapeNewReleasesHandlerTests : IntegrationTestBase
{
    [Fact]
    public async Task Calls_Scraper_for_each_media_sent()
    {
        await Given.TheDatabase.IsSeeded();
        var mediaToScrape = Given.The.Media.MediaList
            .Take(2)
            .Select(media => media.Name)
            .ToList();
        var scrapeNewReleasesCommand = new ScrapeNewReleasesCommand(mediaToScrape);
        Given.TheScraper.ScrapeForAnyMediaReturns(
            Result<ScrapedMediaRelease>.Failure(Errors.Scraper.ScrapeFailedError("")));

        await When.TheApplication.ReceivesCommand<ScrapeNewReleasesCommand, Result>(scrapeNewReleasesCommand);

        Then.TheScraper.HasBeenCalledXTimes(mediaToScrape.Count);
    }

    [Fact]
    public async Task Calls_Scraper_for_all_media_if_sent_empty()
    {
        await Given.TheDatabase.IsSeeded();
        var mediaToScrape = new List<string>();
        var scrapeNewReleasesCommand = new ScrapeNewReleasesCommand(mediaToScrape);
        Given.TheScraper.ScrapeForAnyMediaReturns(
            Result<ScrapedMediaRelease>.Failure(Errors.Scraper.ScrapeFailedError("")));

        await When.TheApplication.ReceivesCommand<ScrapeNewReleasesCommand, Result>(scrapeNewReleasesCommand);

        Then.TheScraper.HasBeenCalledXTimes(Given.The.Media.MediaList.Count);
    }

    [Fact]
    public async Task Fails_if_media_that_does_not_exist_is_sent()
    {
        await Given.TheDatabase.IsSeeded();
        var mediaToScrape = new List<string> { "non existent" };
        var scrapeNewReleasesCommand = new ScrapeNewReleasesCommand(mediaToScrape);
        Given.TheScraper.ScrapeForAnyMediaReturns(
            Result<ScrapedMediaRelease>.Failure(Errors.Scraper.ScrapeFailedError("")));

        var scrapeResult =
            await When.TheApplication.ReceivesCommand<ScrapeNewReleasesCommand, Result>(scrapeNewReleasesCommand);

        Then.TheResult(scrapeResult).IsAFailure();
        Then.TheResult(scrapeResult).ContainsErrorWithCode(Errors.General.NotFoundErrorCode);
    }

    [Fact]
    public async Task Publishes_any_scraped_release_if_no_release_yet()
    {
        await Given.TheDatabase.IsSeeded();
        var mediaName = Given.A.Media.WithSubscriberWithoutReleases.Name;
        var scrapeResult = new ScrapedMediaRelease(
            mediaName,
            "https://www.test.com/chapter/1",
            1
        );
        Given.TheScraper.ScrapeForMediaWithNameReturns(mediaName, Result<ScrapedMediaRelease>.Success(scrapeResult));
        Given.TheNotificationService.NotifyReturns(Result.Success());
        var scrapeNewReleasesCommand = new ScrapeNewReleasesCommand(new List<string> { mediaName });

        var scrapeNewReleasesResult =
            await When.TheApplication.ReceivesCommand<ScrapeNewReleasesCommand, Result>(scrapeNewReleasesCommand);

        Then.TheResult(scrapeNewReleasesResult).IsSuccessful();
        var media = await Then.TheDatabase.Query(unitOfWork => unitOfWork.MediaRepository.GetByName(mediaName));
        media.Should().NotBeNull();
        media!.NewestRelease.Should().NotBeNull();
        media.NewestRelease!.ReleaseNumber.Should().Be(ReleaseNumber.Create(1, 0).Value);
    }

    [Fact]
    public async Task Publishes_release_if_release_is_newer()
    {
        await Given.TheDatabase.IsSeeded();
        var media = Given.A.Media.WithSubscriberWithRelease;
        var mediaName = media.Name;
        var newReleaseNumber = media.NewestRelease!.ReleaseNumber.Major + 1;
        var scrapeResult = new ScrapedMediaRelease(
            mediaName,
            "https://www.test.com/chapter/1",
            newReleaseNumber
        );
        Given.TheScraper.ScrapeForMediaWithNameReturns(mediaName, Result<ScrapedMediaRelease>.Success(scrapeResult));
        var scrapeNewReleasesCommand = new ScrapeNewReleasesCommand(new List<string> { mediaName });

        var scrapeNewReleasesResult =
            await When.TheApplication.ReceivesCommand<ScrapeNewReleasesCommand, Result>(scrapeNewReleasesCommand);

        Then.TheResult(scrapeNewReleasesResult).IsSuccessful();
        var mediaWithNewRelease =
            await Then.TheDatabase.Query(unitOfWork => unitOfWork.MediaRepository.GetByName(mediaName));
        mediaWithNewRelease.Should().NotBeNull();
        mediaWithNewRelease!.NewestRelease.Should().NotBeNull();
        mediaWithNewRelease.NewestRelease!.ReleaseNumber.Should().Be(ReleaseNumber.Create(newReleaseNumber, 0).Value);
    }

    [Fact]
    public async Task Does_not_publish_release_if_it_is_not_newer()
    {
        await Given.TheDatabase.IsSeeded();
        var media = Given.A.Media.WithSubscriberWithRelease;
        var mediaName = media.Name;
        var newReleaseNumber = media.NewestRelease!.ReleaseNumber.Major;
        var scrapeResult = new ScrapedMediaRelease(
            mediaName,
            "https://www.test.com/chapter/1",
            newReleaseNumber
        );
        Given.TheScraper.ScrapeForMediaWithNameReturns(mediaName, Result<ScrapedMediaRelease>.Success(scrapeResult));
        var scrapeNewReleasesCommand = new ScrapeNewReleasesCommand(new List<string> { mediaName });

        var scrapeNewReleasesResult =
            await When.TheApplication.ReceivesCommand<ScrapeNewReleasesCommand, Result>(scrapeNewReleasesCommand);

        Then.TheResult(scrapeNewReleasesResult).IsSuccessful();
        var mediaWithNewRelease =
            await Then.TheDatabase.Query(unitOfWork => unitOfWork.MediaRepository.GetByName(mediaName));
        mediaWithNewRelease.Should().NotBeNull();
        mediaWithNewRelease!.NewestRelease.Should().NotBeNull();
        mediaWithNewRelease.NewestRelease!.ReleaseNumber.Should().Be(ReleaseNumber.Create(newReleaseNumber, 0).Value);
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
        var scrapeNewReleasesCommand = new ScrapeNewReleasesCommand(new List<string> { mediaName });

        var scrapeNewReleasesResult =
            await When.TheApplication.ReceivesCommand<ScrapeNewReleasesCommand, Result>(scrapeNewReleasesCommand);

        Then.TheResult(scrapeNewReleasesResult).IsSuccessful();
        Then.TheNotificationService.HasNotBeenCalled();
    }

    [Fact]
    public async Task Calls_notificationService_if_subscribers_are_found()
    {
        await Given.TheDatabase.IsSeeded();
        var mediaName = Given.A.Media.WithSubscriberWithoutRelease.Name;
        var subscriber = Given.A.Subscriber.WithSubscriptions;
        var linkToReleasedResource = "https://www.test.com/chapter/1";
        var scrapeResult = new ScrapedMediaRelease(
            mediaName,
            linkToReleasedResource,
            1
        );
        Given.TheScraper.ScrapeForMediaWithNameReturns(mediaName, Result<ScrapedMediaRelease>.Success(scrapeResult));
        Given.TheNotificationService.NotifyReturns(Result.Success());
        var scrapeNewReleasesCommand = new ScrapeNewReleasesCommand(new List<string> { mediaName });

        var scrapeNewReleasesResult =
            await When.TheApplication.ReceivesCommand<ScrapeNewReleasesCommand, Result>(scrapeNewReleasesCommand);

        Then.TheResult(scrapeNewReleasesResult).IsSuccessful();
        var expectedReleasePublishedNotification =
            new ReleasePublishedNotification(subscriber.ExternalIdentifier, mediaName, linkToReleasedResource);
        Then.TheNotificationService.HasBeenCalledWithReleasePublishedNotificationOnce(expectedReleasePublishedNotification);
    }
}