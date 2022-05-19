using System.Linq;
using System.Threading.Tasks;
using Application.Ports.Scraper;
using Application.Test.Fixture;
using Application.UseCases.Media.AddMedia;
using Domain.ApplicationErrors;
using Domain.Results;
using FluentAssertions;
using Xunit;

namespace Application.Test.UseCases;

public class AddMediaHandlerTests : IntegrationTestBase
{
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Adding_media_fails_if_website_does_not_exist()
    {
        await Given.TheDatabase.IsSeeded();
        var addMediaCommand = new AddMediaCommand("does not exist", "");

        var addMediaResult = await When.TheApplication.ReceivesCommand<AddMediaCommand, Result>(addMediaCommand);

        Then.TheResult(addMediaResult)
            .IsAFailure()
            .Should()
            .BeTrue();
        Then.TheResult(addMediaResult)
            .ContainsErrorWithCode(Errors.General.NotFoundErrorCode)
            .Should()
            .BeTrue();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Adding_media_does_not_call_scraper_if_website_does_not_exist()
    {
        await Given.TheDatabase.IsSeeded();
        var addMediaCommand = new AddMediaCommand("does not exist", "");

        await When.TheApplication.ReceivesCommand<AddMediaCommand, Result>(addMediaCommand);

        Then.TheMediaNameScraper.HasBeenCalledXTimes(0);
        Then.TheScraper.HasBeenCalledXTimes(0);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Adding_media_fails_if_mediaName_can_not_be_scraped()
    {
        await Given.TheDatabase.IsSeeded();
        Given.TheMediaNameScraper.ScrapeForAnyMediaReturns(
            Result<ScrapedMediaName>.Failure(Errors.Scraper.ScrapeMediaNameFailedError(""))
        );
        var mediaToAdd = Given.The.Media.NotPersistedMedia;
        var addMediaCommand = new AddMediaCommand(
            Given.The.Website.EarlyManga.Name,
            mediaToAdd.ScrapeTargets.First().RelativeUrl
        );
        
        var addMediaResult = await When.TheApplication.ReceivesCommand<AddMediaCommand, Result>(addMediaCommand);

        Then.TheResult(addMediaResult)
            .IsAFailure()
            .Should()
            .BeTrue();
        Then.TheResult(addMediaResult)
            .ContainsErrorWithCode(Errors.Scraper.ScrapeMediaNameFailedErrorCode)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Adding_media_fails_if_scraper_can_not_scrape_a_release()
    {
        await Given.TheDatabase.IsSeeded();
        var mediaToAdd = Given.The.Media.NotPersistedMedia;
        var earlyManga = Given.The.Website.EarlyManga;
        var addMediaCommand = new AddMediaCommand(
            earlyManga.Name,
            mediaToAdd.ScrapeTargets.First().RelativeUrl
        );
        Given.TheMediaNameScraper.ScrapeForAnyMediaReturns(
            Result<ScrapedMediaName>.Success(new ScrapedMediaName(mediaToAdd.Name))
        );
        var scrapedMediaRelease = new ScrapedMediaRelease(
            $"{earlyManga.Url}tower-of-god/chapter-541",
            541,
            0
        );
        Given.TheScraper.ScrapeForAnyMediaReturns(
            Result<ScrapedMediaRelease>.Failure(Errors.Scraper.ScrapeFailedError(""))
        );
        
        var addMediaResult = await When.TheApplication.ReceivesCommand<AddMediaCommand, Result>(addMediaCommand);

        Then.TheResult(addMediaResult)
            .IsAFailure()
            .Should()
            .BeTrue();
        Then.TheResult(addMediaResult)
            .ContainsErrorWithCode(Errors.Scraper.ScrapeFailedErrorCode)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Media_is_persisted_if_scraping_mediaName_and_release_succeed()
    {
        await Given.TheDatabase.IsSeeded();
        var mediaToAdd = Given.The.Media.NotPersistedMedia;
        var earlyManga = Given.The.Website.EarlyManga;
        var addMediaCommand = new AddMediaCommand(
            earlyManga.Name,
            mediaToAdd.ScrapeTargets.First().RelativeUrl
        );
        Given.TheMediaNameScraper.ScrapeForAnyMediaReturns(
            Result<ScrapedMediaName>.Success(new ScrapedMediaName(mediaToAdd.Name))
        );
        var scrapedMediaRelease = new ScrapedMediaRelease(
            $"{earlyManga.Url}tower-of-god/chapter-541",
            541,
            0
        );
        Given.TheScraper.ScrapeForMediaWithNameReturns(
            mediaToAdd.Name,
            Result<ScrapedMediaRelease>.Success(scrapedMediaRelease)
        );
        
        var addMediaResult = await When.TheApplication.ReceivesCommand<AddMediaCommand, Result>(addMediaCommand);

        Then.TheResult(addMediaResult)
            .IsSuccessful()
            .Should()
            .BeTrue();
        
        var media =
            await Then.TheDatabase.Query(unitOfWork => unitOfWork.MediaRepository.GetByName(mediaToAdd.Name));
        media.Should().NotBeNull();
        media.Name.Should().Be(mediaToAdd.Name);
        media.ScrapeTargets.Should().NotBeNull();
        media.ScrapeTargets.Should().HaveCount(1);
        var scrapeTarget = media.ScrapeTargets.First();
        scrapeTarget.WebsiteId.Should().Be(earlyManga.Id);
        scrapeTarget.RelativeUrl.Should().Be(mediaToAdd.ScrapeTargets.First().RelativeUrl);
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Adding_Media_fails_if_media_with_ScrapeTarget_already_exists()
    {
        await Given.TheDatabase.IsSeeded();
        var alreadyPersistedMedia = Given.The.Media.WithoutSubscriberWithoutReleases;
        var earlyManga = Given.The.Website.EarlyManga;
        var addMediaCommand = new AddMediaCommand(
            earlyManga.Name,
            alreadyPersistedMedia.ScrapeTargets.First().RelativeUrl
        );
        Given.TheMediaNameScraper.ScrapeForAnyMediaReturns(
            Result<ScrapedMediaName>.Success(new ScrapedMediaName(alreadyPersistedMedia.Name))
        );
        var scrapedMediaRelease = new ScrapedMediaRelease(
            $"{earlyManga.Url}tower-of-god/chapter-541",
            541,
            0
        );
        Given.TheScraper.ScrapeForMediaWithNameReturns(
            alreadyPersistedMedia.Name,
            Result<ScrapedMediaRelease>.Success(scrapedMediaRelease)
        );

        var addMediaResult = await When.TheApplication.ReceivesCommand<AddMediaCommand, Result>(addMediaCommand);

        Then.TheResult(addMediaResult)
            .IsSuccessful()
            .Should()
            .BeFalse();
        Then.TheResult(addMediaResult)
            .ContainsErrorWithCode(Errors.Media.MediaWithScrapeTargetExistsErrorCode)
            .Should()
            .BeTrue();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Adding_Media_fails_if_media_with_name_already_exists()
    {
        await Given.TheDatabase.IsSeeded();
        var alreadyPersistedMedia = Given.The.Media.WithoutSubscriberWithoutReleases;
        var earlyManga = Given.The.Website.EarlyManga;
        var scrapeTarget = alreadyPersistedMedia.ScrapeTargets.First();
        var deviatingRelativeUrl = scrapeTarget.RelativeUrl + "/differs";
        var addMediaCommand = new AddMediaCommand(
            earlyManga.Name,
            deviatingRelativeUrl
        );
        Given.TheMediaNameScraper.ScrapeForAnyMediaReturns(
            Result<ScrapedMediaName>.Success(new ScrapedMediaName(alreadyPersistedMedia.Name))
        );
        var scrapedMediaRelease = new ScrapedMediaRelease(
            $"{earlyManga.Url}tower-of-god/chapter-541",
            541,
            0
        );
        Given.TheScraper.ScrapeForMediaWithNameReturns(
            alreadyPersistedMedia.Name,
            Result<ScrapedMediaRelease>.Success(scrapedMediaRelease)
        );

        var addMediaResult = await When.TheApplication.ReceivesCommand<AddMediaCommand, Result>(addMediaCommand);

        Then.TheResult(addMediaResult)
            .IsSuccessful()
            .Should()
            .BeFalse();
        Then.TheResult(addMediaResult)
            .ContainsErrorWithCode(Errors.Media.MediaWithNameExistsErrorCode)
            .Should()
            .BeTrue();
    }

   
    
    
}