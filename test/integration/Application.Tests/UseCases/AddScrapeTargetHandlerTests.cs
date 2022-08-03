using System.Linq;
using System.Threading.Tasks;
using Application.Ports.Scraper;
using Application.Test.Fixture;
using Application.UseCases.Media.AddScrapeTarget;
using Application.UseCases.Media.QueryScrapeTargets;
using Domain.ApplicationErrors;
using Domain.Results;
using FluentAssertions;
using Xunit;

namespace Application.Test.UseCases;

public class AddScrapeTargetHandlerTests : IntegrationTestBase
{
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Fails_if_media_does_not_exist()
    {
        var addScrapeTargetCommand = new AddScrapeTargetCommand(
            "does_not_exist",
            Given.The.Website.EarlyManga.Name,
            Given.The.ScrapeTarget.MartialPeakEarlyManga.RelativeUrl.Value);

        var addScrapeTargetResult = await When.TheApplication
            .ReceivesCommand<AddScrapeTargetCommand, Result>(addScrapeTargetCommand);

        Then.TheResult(addScrapeTargetResult)
            .IsSuccessful()
            .Should()
            .BeFalse();
        Then.TheResult(addScrapeTargetResult)
            .ContainsErrorWithCode(Errors.General.NotFoundErrorCode);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Fails_if_website_does_not_exist()
    {
        await Given.TheDatabase.IsSeeded();
        var addScrapeTargetCommand = new AddScrapeTargetCommand(
            Given.A.Media.WithSubscriberWithoutRelease.Name,
            "does_not_exist",
            Given.The.ScrapeTarget.MartialPeakEarlyManga.RelativeUrl.Value);

        var addScrapeTargetResult = await When.TheApplication
            .ReceivesCommand<AddScrapeTargetCommand, Result>(addScrapeTargetCommand);

        Then.TheResult(addScrapeTargetResult)
            .IsSuccessful()
            .Should()
            .BeFalse();
        Then.TheResult(addScrapeTargetResult)
            .ContainsErrorWithCode(Errors.General.NotFoundErrorCode);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Fails_if_ScrapeTarget_already_configured()
    {
        await Given.TheDatabase.IsSeeded();
        var mediaWithScrapeTarget = Given.A.Media.MediaWithScrapeTargets.First();
        var scrapeTarget = mediaWithScrapeTarget.ScrapeTargets.First();
        var website = Given.The.Website.ActiveWebsites.Single(website => website.Id == scrapeTarget.WebsiteId);
        var addScrapeTargetCommand = new AddScrapeTargetCommand(
            mediaWithScrapeTarget.Name,
            website.Name,
            scrapeTarget.RelativeUrl.Value
        );

        var addScrapeTargetResult = await When.TheApplication
            .ReceivesCommand<AddScrapeTargetCommand, Result>(addScrapeTargetCommand);

        Then.TheResult(addScrapeTargetResult)
            .IsSuccessful()
            .Should()
            .BeFalse();
        Then.TheResult(addScrapeTargetResult)
            .ContainsErrorWithCode(Errors.Media.ScrapeTargetExistsErrorCode);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Fails_if_scraping_does_not_succeed()
    {
        await Given.TheDatabase.IsSeeded();
        Given.TheScraper.ScrapeForAnyMediaReturns(
            Result<ScrapedMediaRelease>.Failure(Errors.Scraper.ScrapeFailedError(""))
        );
        var addScrapeTargetCommand = new AddScrapeTargetCommand(
            Given.A.Media.WithoutSubscribersWithoutReleasesWithoutScrapeTarget.Name,
            Given.The.Website.EarlyManga.Name,
            "/some/relative/url/");

        var addScrapeTargetResult = await When.TheApplication
            .ReceivesCommand<AddScrapeTargetCommand, Result>(addScrapeTargetCommand);

        Then.TheResult(addScrapeTargetResult)
            .IsSuccessful()
            .Should()
            .BeFalse();
        Then.TheResult(addScrapeTargetResult)
            .ContainsErrorWithCode(Errors.Scraper.ScrapeFailedErrorCode);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Succeeds_if_media_has_no_ScrapeTargets_yet()
    {
        await Given.TheDatabase.IsSeeded();
        var mediaWithoutScrapeTarget = Given.A.Media.WithoutSubscribersWithoutReleasesWithoutScrapeTarget;
        var website = Given.The.Website.EarlyManga;
        var scrapeTarget = Given.The.ScrapeTarget.BorutoEarlyManga;
        Given.TheScraper.ScrapeForAnyMediaReturns(Result<ScrapedMediaRelease>.Success(
                new ScrapedMediaRelease(
                    website.Url.Value + scrapeTarget.RelativeUrl.Value,
                    1
                )
            )
        );

        var addScrapeTargetCommand = new AddScrapeTargetCommand(
            mediaWithoutScrapeTarget.Name,
            website.Name,
            scrapeTarget.RelativeUrl.Value);

        var addScrapeTargetResult = await When.TheApplication
            .ReceivesCommand<AddScrapeTargetCommand, Result>(addScrapeTargetCommand);

        Then.TheResult(addScrapeTargetResult)
            .IsSuccessful()
            .Should()
            .BeTrue();

        var scrapeTargetsQuery = new ScrapeTargetsQuery(mediaWithoutScrapeTarget.Name);
        var scrapeTargetsResult =
            await Then.TheApplication.ReceivesQuery<ScrapeTargetsQuery, Result<ScrapeTargets>>(scrapeTargetsQuery);
        Then.TheResult(addScrapeTargetResult)
            .IsSuccessful()
            .Should()
            .BeTrue();
        var scrapeTargets = scrapeTargetsResult.Value;
        scrapeTargets.ScrapeTargetInformation
            .Should()
            .HaveCount(mediaWithoutScrapeTarget.ScrapeTargets.Count + 1);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Succeeds_if_ScrapeTarget_is_new_for_media()
    {
        await Given.TheDatabase.IsSeeded();
        var mediaWithScrapeTarget = Given.A.Media.WithSubscriberWithReleases;
        var website = Given.The.Website.Manganato;
        var scrapeTarget = Given.The.ScrapeTarget.MartialPeakManganato;
        Given.TheScraper.ScrapeForAnyMediaReturns(Result<ScrapedMediaRelease>.Success(
                new ScrapedMediaRelease(
                    website.Url.Value + scrapeTarget.RelativeUrl.Value,
                    1
                )
            )
        );

        var addScrapeTargetCommand = new AddScrapeTargetCommand(
            mediaWithScrapeTarget.Name,
            website.Name,
            scrapeTarget.RelativeUrl.Value);

        var addScrapeTargetResult = await When.TheApplication
            .ReceivesCommand<AddScrapeTargetCommand, Result>(addScrapeTargetCommand);

        Then.TheResult(addScrapeTargetResult)
            .IsSuccessful()
            .Should()
            .BeTrue();

        var scrapeTargetsQuery = new ScrapeTargetsQuery(mediaWithScrapeTarget.Name);
        var scrapeTargetsResult =
            await Then.TheApplication.ReceivesQuery<ScrapeTargetsQuery, Result<ScrapeTargets>>(scrapeTargetsQuery);
        Then.TheResult(addScrapeTargetResult)
            .IsSuccessful()
            .Should()
            .BeTrue();
        var scrapeTargets = scrapeTargetsResult.Value;
        scrapeTargets.ScrapeTargetInformation
            .Should()
            .HaveCount(mediaWithScrapeTarget.ScrapeTargets.Count + 1);
    }
}