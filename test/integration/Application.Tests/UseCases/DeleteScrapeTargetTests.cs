using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Test.Fixture;
using Application.UseCases.Media.DeleteScrapeTarget;
using Domain.ApplicationErrors;
using Domain.Results;
using FluentAssertions;
using Shared;
using Xunit;

namespace Application.Test.UseCases;

public class DeleteScrapeTargetTests : IntegrationTestBase
{
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Fails_if_logged_in_user_is_no_admin()
    {
        await Given.TheDatabase.IsSeeded();
        var media = Given.A.Media.WithSubscriberWithReleases;
        var scrapeTargetToRemove = media.ScrapeTargets.First();
        var deleteScrapeTargetCommand = new DeleteScrapeTargetCommand(
            media.Id,
            scrapeTargetToRemove.Id,
            GivenTheExternalIdentifier.NonAdminIdentifier
        );
        
        var deleteScrapeTargetResult = await When.TheApplication
            .ReceivesCommand<DeleteScrapeTargetCommand, Result>(deleteScrapeTargetCommand);

        Then.TheResult(deleteScrapeTargetResult)
            .IsSuccessful()
            .Should()
            .BeFalse();
        Then.TheResult(deleteScrapeTargetResult)
            .ContainsErrorWithCode(Errors.Authorization.AdminRightsMissingErrorCode)
            .Should()
            .BeTrue();
        var mediaResult = await Then.TheDatabase.GetMediaById(media.Id);
        mediaResult.Should().NotBeNull();
        mediaResult.ScrapeTargets.Should().Contain(scrapeTargetToRemove);
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Fails_if_ScrapeTarget_does_not_exist()
    {
        await Given.TheDatabase.IsSeeded();
        var media = Given.A.Media.WithSubscriberWithReleases;
        var scrapeTargetIdThatDoesNotExist = Guid.NewGuid();
        var deleteScrapeTargetCommand = new DeleteScrapeTargetCommand(
            media.Id,
            scrapeTargetIdThatDoesNotExist,
            GivenTheExternalIdentifier.AdminIdentifier
        );
        
        var deleteScrapeTargetResult = await When.TheApplication
            .ReceivesCommand<DeleteScrapeTargetCommand, Result>(deleteScrapeTargetCommand);

        Then.TheResult(deleteScrapeTargetResult)
            .IsSuccessful()
            .Should()
            .BeFalse();
        Then.TheResult(deleteScrapeTargetResult)
            .ContainsErrorWithCode(Errors.General.NotFoundErrorCode)
            .Should()
            .BeTrue();
        var mediaResult = await Then.TheDatabase.GetMediaById(media.Id);
        mediaResult.Should().NotBeNull();
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Fails_if_Media_does_not_exist()
    {
        var mediaThatDoesNotExist = GivenTheMedia.Create().Value;
        var scrapeTargetIdThatDoesNotExist = Guid.NewGuid();
        var deleteScrapeTargetCommand = new DeleteScrapeTargetCommand(
            mediaThatDoesNotExist.Id,
            scrapeTargetIdThatDoesNotExist,
            GivenTheExternalIdentifier.AdminIdentifier
        );
        
        var deleteScrapeTargetResult = await When.TheApplication
            .ReceivesCommand<DeleteScrapeTargetCommand, Result>(deleteScrapeTargetCommand);

        Then.TheResult(deleteScrapeTargetResult)
            .IsSuccessful()
            .Should()
            .BeFalse();
        Then.TheResult(deleteScrapeTargetResult)
            .ContainsErrorWithCode(Errors.General.NotFoundErrorCode)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Succeeds_if_ScrapeTarget_exists()
    {
        await Given.TheDatabase.IsSeeded();
        var media = Given.A.Media.WithSubscriberWithReleases;
        var scrapeTargetToRemove = media.ScrapeTargets.First();
        var deleteScrapeTargetCommand = new DeleteScrapeTargetCommand(
            media.Id,
            scrapeTargetToRemove.Id,
            GivenTheExternalIdentifier.AdminIdentifier
        );
        
        var deleteScrapeTargetResult = await When.TheApplication
            .ReceivesCommand<DeleteScrapeTargetCommand, Result>(deleteScrapeTargetCommand);

        Then.TheResult(deleteScrapeTargetResult)
            .IsSuccessful()
            .Should()
            .BeTrue();
        var mediaResult = await Then.TheDatabase.GetMediaById(media.Id);
        mediaResult.Should().NotBeNull();
        mediaResult.ScrapeTargets.Should().NotContain(scrapeTargetToRemove);
    }
}