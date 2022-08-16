using System;
using System.Threading.Tasks;
using Application.Test.Fixture;
using Application.UseCases.Media.DeleteMedia;
using Domain.ApplicationErrors;
using Domain.Results;
using FluentAssertions;
using Shared;
using Xunit;

namespace Application.Test.UseCases;

public class DeleteMediaTests : IntegrationTestBase
{
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Fails_if_logged_in_user_is_no_admin()
    {
        await Given.TheDatabase.IsSeeded();
        var media = Given.A.Media.WithSubscriberWithReleases;
        var deleteMediaCommand = new DeleteMediaCommand(
            media.Id,
            GivenTheExternalIdentifier.NonAdminIdentifier
        );
        
        var deleteMediaResult = await When.TheApplication
            .ReceivesCommand<DeleteMediaCommand, Result>(deleteMediaCommand);

        Then.TheResult(deleteMediaResult)
            .IsSuccessful()
            .Should()
            .BeFalse();
        Then.TheResult(deleteMediaResult)
            .ContainsErrorWithCode(Errors.Authorization.AdminRightsMissingErrorCode)
            .Should()
            .BeTrue();
        var mediaResult = await Then.TheDatabase.GetMediaById(media.Id);
        mediaResult.Should().NotBeNull();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Fails_if_Media_does_not_exist()
    {
        var deleteMediaCommand = new DeleteMediaCommand(
            Guid.NewGuid(),
            GivenTheExternalIdentifier.AdminIdentifier
        );
        
        var deleteMediaResult = await When.TheApplication
            .ReceivesCommand<DeleteMediaCommand, Result>(deleteMediaCommand);

        Then.TheResult(deleteMediaResult)
            .IsSuccessful()
            .Should()
            .BeFalse();
        Then.TheResult(deleteMediaResult)
            .ContainsErrorWithCode(Errors.General.NotFoundErrorCode)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Succeeds_if_Media_exists()
    {
        await Given.TheDatabase.IsSeeded();
        var media = Given.A.Media.WithSubscriberWithReleases;
        var subscriber = Given.A.Subscriber.WithSubscriptions;
        var deleteMediaCommand = new DeleteMediaCommand(
            media.Id,
            GivenTheExternalIdentifier.AdminIdentifier
        );
        
        var deleteMediaResult = await When.TheApplication
            .ReceivesCommand<DeleteMediaCommand, Result>(deleteMediaCommand);

        Then.TheResult(deleteMediaResult)
            .IsSuccessful()
            .Should()
            .BeTrue();
        var mediaResult = await Then.TheDatabase.GetMediaById(media.Id);
        mediaResult.Should().BeNull();
        var subscriberResult = await Then.TheDatabase.GetSubscriberByExternalId(subscriber.ExternalIdentifier);
        subscriberResult.Should().NotBeNull();
        subscriberResult.SubscribedToMediaIds.Should().NotContain(media.Id);
    }
}