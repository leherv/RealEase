using System.Linq;
using System.Threading.Tasks;
using Application.Test.Fixture;
using Application.UseCases.Media.QueryScrapeTargets;
using Domain.ApplicationErrors;
using Domain.Results;
using FluentAssertions;
using Xunit;

namespace Application.Test.UseCases;

public class QueryScrapeTargetsHandlerTests : IntegrationTestBase
{
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Fails_if_media_with_mediaName_does_not_exist()
    {
        var scrapeTargetsQuery = new ScrapeTargetsQuery("does_not_exist");

        var scrapeTargetsResult = await When.TheApplication.ReceivesQuery<ScrapeTargetsQuery, Result<ScrapeTargets>>(scrapeTargetsQuery);

        Then.TheResult(scrapeTargetsResult)
            .IsSuccessful()
            .Should()
            .BeFalse();
        Then.TheResult(scrapeTargetsResult)
            .ContainsErrorWithCode(Errors.General.NotFoundErrorCode);
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Returns_non_empty_list_if_media_exists_and_has_ScrapeTargets()
    {
        await Given.TheDatabase.IsSeeded();
        var media = Given.A.Media.MediaWithScrapeTargets.First();
        var scrapeTargetsQuery = new ScrapeTargetsQuery(media.Name);
        
        var scrapeTargetsResult = await When.TheApplication.ReceivesQuery<ScrapeTargetsQuery, Result<ScrapeTargets>>(scrapeTargetsQuery);
        
        Then.TheResult(scrapeTargetsResult)
            .IsSuccessful()
            .Should()
            .BeTrue();
        var scrapeTargets = scrapeTargetsResult.Value;
        scrapeTargets.ScrapeTargetInformation
            .Should()
            .HaveCount(media.ScrapeTargets.Count);
        var scrapeTarget = media.ScrapeTargets.First();
        var scrapeTargetInformation = scrapeTargets.ScrapeTargetInformation.First();
        scrapeTargetInformation.RelativeUrl
            .Should()
            .Be(scrapeTarget.RelativeUrl.Value);
        scrapeTargetInformation.WebsiteName
            .Should()
            .NotBeNullOrWhiteSpace();
        scrapeTargetInformation.WebsiteUrl
            .Should()
            .NotBeNullOrWhiteSpace();
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Returns_empty_list_if_media_exists_but_has_no_ScrapeTargets_yet()
    {
        await Given.TheDatabase.IsSeeded();
        var media = Given.A.Media.WithoutSubscribersWithoutReleasesWithoutScrapeTarget;
        var scrapeTargetsQuery = new ScrapeTargetsQuery(media.Name);
        
        var scrapeTargetsResult = await When.TheApplication.ReceivesQuery<ScrapeTargetsQuery, Result<ScrapeTargets>>(scrapeTargetsQuery);
        
        Then.TheResult(scrapeTargetsResult)
            .IsSuccessful()
            .Should()
            .BeTrue();
        var scrapeTargets = scrapeTargetsResult.Value;
        scrapeTargets.ScrapeTargetInformation
            .Should()
            .HaveCount(media.ScrapeTargets.Count);
        var scrapeTarget = media.ScrapeTargets.FirstOrDefault();
        var scrapeTargetInformation = scrapeTargets.ScrapeTargetInformation.FirstOrDefault();
        scrapeTarget
            .Should()
            .BeNull();
        scrapeTargetInformation
            .Should()
            .BeNull();
    }
}