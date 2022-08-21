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
using FluentAssertions;
using Xunit;

namespace Application.Test.UseCases;

public class ScrapeNewReleasesHandlerTests : IntegrationTestBase
{
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Calls_Scraper_for_each_media_sent()
    {
        await Given.TheDatabase.IsSeeded();
        var mediaToScrape = Given.The.Media.PersistedMediaList
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
    [Trait("Category", "Integration")]
    public async Task Calls_Scraper_for_each_ScrapeTarget()
    {
        await Given.TheDatabase.IsSeeded();
        var mediaToScrape = Given.A.Media.WithSubscriberWithTwoScrapeTargets;
        var scrapeNewReleasesCommand = new ScrapeNewReleasesCommand(new[] { mediaToScrape.Name });
        Given.TheScraper.ScrapeForAnyMediaReturns(
            Result<ScrapedMediaRelease>.Failure(Errors.Scraper.ScrapeFailedError("")));

        await When.TheApplication.ReceivesCommand<ScrapeNewReleasesCommand, Result>(scrapeNewReleasesCommand);

        Then.TheScraper.HasBeenCalledXTimes(mediaToScrape.ScrapeTargets.Count);
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Does_not_call_scraper_if_website_is_not_active()
    {
        await Given.TheDatabase.IsSeeded();
        var mediaToScrape = Given.A.Media.WithInActiveWebsite;
        var scrapeNewReleasesCommand = new ScrapeNewReleasesCommand(new[] { mediaToScrape.Name });
        Given.TheScraper.ScrapeForAnyMediaReturns(
            Result<ScrapedMediaRelease>.Failure(Errors.Scraper.ScrapeFailedError("")));

        await When.TheApplication.ReceivesCommand<ScrapeNewReleasesCommand, Result>(scrapeNewReleasesCommand);

        Then.TheScraper.HasBeenCalledXTimes(0);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Calls_Scraper_for_all_media_if_sent_empty()
    {
        await Given.TheDatabase.IsSeeded();
        var mediaToScrape = new List<string>();
        var scrapeNewReleasesCommand = new ScrapeNewReleasesCommand(mediaToScrape);
        Given.TheScraper.ScrapeForAnyMediaReturns(
            Result<ScrapedMediaRelease>.Failure(Errors.Scraper.ScrapeFailedError("")));

        await When.TheApplication.ReceivesCommand<ScrapeNewReleasesCommand, Result>(scrapeNewReleasesCommand);

        Then.TheScraper.HasBeenCalledXTimes(Given.The.Media.MediaWithScrapeTargets
            .SelectMany(media => media.ScrapeTargets)
            .Count()
        );
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Does_not_call_Scraper_if_no_ScrapeTarget_set()
    {
        await Given.TheDatabase.IsSeeded();
        var media = Given.A.Media.WithoutSubscribersWithoutReleasesWithoutScrapeTarget;
        var scrapeNewReleasesCommand = new ScrapeNewReleasesCommand(new[] { media.Name });

        await When.TheApplication.ReceivesCommand<ScrapeNewReleasesCommand, Result>(scrapeNewReleasesCommand);

        Then.TheScraper.HasBeenCalledXTimes(0);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Does_not_call_NotificationService_if_no_ScrapeTarget_set()
    {
        await Given.TheDatabase.IsSeeded();
        var media = Given.A.Media.WithoutSubscribersWithoutReleasesWithoutScrapeTarget;
        var scrapeNewReleasesCommand = new ScrapeNewReleasesCommand(new[] { media.Name });

        await When.TheApplication.ReceivesCommand<ScrapeNewReleasesCommand, Result>(scrapeNewReleasesCommand);

        Then.TheNotificationService.HasNotBeenCalledForMediaWithName(media.Name);
    }

    [Fact]
    [Trait("Category", "Integration")]
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
        Then.TheResult(scrapeResult)
            .ContainsErrorWithCode(Errors.General.NotFoundErrorCode)
            .Should()
            .BeTrue();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Publishes_any_scraped_release_if_no_release_yet()
    {
        await Given.TheDatabase.IsSeeded();
        var mediaName = Given.A.Media.WithoutSubscriberWithoutReleases.Name;
        var scrapeResult = new ScrapedMediaRelease(
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
    [Trait("Category", "Integration")]
    public async Task Publishes_release_if_release_is_newer()
    {
        await Given.TheDatabase.IsSeeded();
        var media = Given.A.Media.WithSubscriberWithReleases;
        var mediaName = media.Name;
        var newReleaseNumber = media.NewestRelease!.ReleaseNumber.Major + 1;
        var scrapeResult = new ScrapedMediaRelease(
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
    [Trait("Category", "Integration")]
    public async Task Does_not_publish_release_if_it_is_not_newer()
    {
        await Given.TheDatabase.IsSeeded();
        var media = Given.A.Media.WithSubscriberWithReleases;
        var mediaName = media.Name;
        var newReleaseNumber = media.NewestRelease!.ReleaseNumber.Major;
        var scrapeResult = new ScrapedMediaRelease(
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
    [Trait("Category", "Integration")]
    public async Task Does_not_call_notificationService_if_no_subscribers()
    {
        await Given.TheDatabase.IsSeeded();
        var mediaName = Given.A.Media.WithoutSubscriberWithoutReleases.Name;
        var scrapeResult = new ScrapedMediaRelease(
            "https://www.test.com/chapter/1",
            1
        );
        Given.TheScraper.ScrapeForMediaWithNameReturns(mediaName, Result<ScrapedMediaRelease>.Success(scrapeResult));
        var scrapeNewReleasesCommand = new ScrapeNewReleasesCommand(new List<string> { mediaName });

        var scrapeNewReleasesResult =
            await When.TheApplication.ReceivesCommand<ScrapeNewReleasesCommand, Result>(scrapeNewReleasesCommand);

        Then.TheResult(scrapeNewReleasesResult).IsSuccessful();
        Then.TheNotificationService.HasNotBeenCalledForMediaWithName(mediaName);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Calls_notificationService_if_subscribers_are_found()
    {
        await Given.TheDatabase.IsSeeded();
        var mediaName = Given.A.Media.WithSubscriberWithoutRelease.Name;
        var subscriber = Given.A.Subscriber.WithSubscriptions;
        var linkToReleasedResource = "https://www.test.com/chapter/1";
        var scrapeResult = new ScrapedMediaRelease(
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
        Then.TheNotificationService.HasBeenCalledWithReleasePublishedNotificationOnce(
            expectedReleasePublishedNotification);
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Chooses_newest_release_if_multiple_are_found()
    {
        await Given.TheDatabase.IsSeeded();
        var media = Given.A.Media.WithSubscriberWithTwoScrapeTargets;
        var subscriber = Given.A.Subscriber.WithSubscriptions;
        
        var website = Given.The.Website.ActiveWebsites.Single(website => website.Id == media.ScrapeTargets.First().WebsiteId);
        var linkToReleasedResource = "https://www.test.com/chapter/1";
        var scrapeResult = new ScrapedMediaRelease(
            linkToReleasedResource,
            1
        );
        Given.TheScraper.ScrapeForWebsiteReturns(website.Name, Result<ScrapedMediaRelease>.Success(scrapeResult));
        
        var website2 = Given.The.Website.ActiveWebsites.Single(website => website.Id == media.ScrapeTargets.Skip(1).First().WebsiteId);
        var linkToReleasedResource2 = "https://www.test2.com/chapter/2";
        var scrapeResult2 = new ScrapedMediaRelease(
            linkToReleasedResource2,
            2
        );
        Given.TheScraper.ScrapeForWebsiteReturns(website2.Name, Result<ScrapedMediaRelease>.Success(scrapeResult2));
        
        Given.TheNotificationService.NotifyReturns(Result.Success());
        var scrapeNewReleasesCommand = new ScrapeNewReleasesCommand(new List<string> { media.Name });

        var scrapeNewReleasesResult =
            await When.TheApplication.ReceivesCommand<ScrapeNewReleasesCommand, Result>(scrapeNewReleasesCommand);

        Then.TheResult(scrapeNewReleasesResult).IsSuccessful();
        var expectedReleasePublishedNotification =
            new ReleasePublishedNotification(subscriber.ExternalIdentifier, media.Name, linkToReleasedResource2);
        Then.TheNotificationService.HasBeenCalledWithReleasePublishedNotificationOnce(
            expectedReleasePublishedNotification);

        var persistedMedia = await Then.TheDatabase.GetMediaByName(media.Name);
        persistedMedia
            .Should()
            .NotBeNull();
        persistedMedia.NewestRelease.ResourceUrl
            .Should()
            .Be(ResourceUrl.Create(linkToReleasedResource2).Value);
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Publish_and_notfiy_are_not_called_if_all_scrapes_unsuccessful()
    {
        await Given.TheDatabase.IsSeeded();
        var media = Given.A.Media.WithSubscriberWithTwoScrapeTargets;
        Given.TheScraper.ScrapeForAnyMediaReturns(Result<ScrapedMediaRelease>.Failure(Errors.Scraper.ScrapeFailedError("")));
        Given.TheNotificationService.NotifyReturns(Result.Success());
        var scrapeNewReleasesCommand = new ScrapeNewReleasesCommand(new List<string> { media.Name });

        var scrapeNewReleasesResult =
            await When.TheApplication.ReceivesCommand<ScrapeNewReleasesCommand, Result>(scrapeNewReleasesCommand);

        Then.TheResult(scrapeNewReleasesResult).IsSuccessful();
        Then.TheNotificationService.HasNotBeenCalledForMediaWithName(media.Name);
        var persistedMedia = await Then.TheDatabase.GetMediaByName(media.Name);
        persistedMedia
            .Should()
            .NotBeNull();
        persistedMedia.NewestRelease
            .Should()
            .BeNull();
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Publishes_the_successful_scrape_if_at_least_one_is_successful_and_no_release_yet()
    {
        await Given.TheDatabase.IsSeeded();
        var media = Given.A.Media.WithSubscriberWithTwoScrapeTargets;
        var subscriber = Given.A.Subscriber.WithSubscriptions;
        
        var website = Given.The.Website.ActiveWebsites.Single(website => website.Id == media.ScrapeTargets.First().WebsiteId);
        Given.TheScraper.ScrapeForWebsiteReturns(website.Name, Result<ScrapedMediaRelease>.Failure(Errors.Scraper.ScrapeFailedError("")));
        
        var website2 = Given.The.Website.ActiveWebsites.Single(website => website.Id == media.ScrapeTargets.Skip(1).First().WebsiteId);
        var linkToReleasedResource2 = "https://www.test2.com/chapter/1";
        var scrapeResult2 = new ScrapedMediaRelease(
            linkToReleasedResource2,
            1
        );
        Given.TheScraper.ScrapeForWebsiteReturns(website2.Name, Result<ScrapedMediaRelease>.Success(scrapeResult2));
        
        Given.TheNotificationService.NotifyReturns(Result.Success());
        var scrapeNewReleasesCommand = new ScrapeNewReleasesCommand(new List<string> { media.Name });

        var scrapeNewReleasesResult =
            await When.TheApplication.ReceivesCommand<ScrapeNewReleasesCommand, Result>(scrapeNewReleasesCommand);

        Then.TheResult(scrapeNewReleasesResult).IsSuccessful();
        var expectedReleasePublishedNotification =
            new ReleasePublishedNotification(subscriber.ExternalIdentifier, media.Name, linkToReleasedResource2);
        Then.TheNotificationService.HasBeenCalledWithReleasePublishedNotificationOnce(
            expectedReleasePublishedNotification);

        var persistedMedia = await Then.TheDatabase.GetMediaByName(media.Name);
        persistedMedia
            .Should()
            .NotBeNull();
        persistedMedia.NewestRelease.ResourceUrl
            .Should()
            .Be(ResourceUrl.Create(linkToReleasedResource2).Value);
        persistedMedia.NewestRelease.ReleaseNumber.Major
            .Should()
            .Be(scrapeResult2.MajorReleaseNumber);
    }
}