using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Test.Fixture;
using Application.UseCases.Subscriber.QueryMediaSubscriptions;
using Application.UseCases.Subscriber.SubscribeMedia;
using Domain.ApplicationErrors;
using Domain.Results;
using FluentAssertions;
using Xunit;

namespace Application.Test.UseCases;

public class SubscribeHandlerTests : IntegrationTestBase
{
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Subscribing_to_new_media_succeeds_when_Subscriber_already_exists()
    {
        await Given.TheDatabase.IsSeeded();
        var subscriber = Given.A.Subscriber.WithoutSubscriptions;
        var mediaToSubscribeTo = Given.A.Media.MediaList.First();
        var subscribeMedia = new SubscribeMediaCommand(subscriber.ExternalIdentifier, mediaToSubscribeTo.Name);

        var subscribeMediaResult = await When.TheApplication.ReceivesCommand<SubscribeMediaCommand, Result>(subscribeMedia);

        Then.TheResult(subscribeMediaResult).IsSuccessful();
        var mediaSubscriptions =
            await Then.TheApplication.ReceivesQuery<MediaSubscriptionsQuery, MediaSubscriptions>(new MediaSubscriptionsQuery(subscriber.ExternalIdentifier));
        mediaSubscriptions.Should().NotBeNull();
        mediaSubscriptions.SubscribedToMediaNames.Should().Contain(mediaToSubscribeTo.Name);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Subscribing_to_new_media_succeeds_when_Subscriber_does_not_exist_yet()
    {
        await Given.TheDatabase.IsSeeded();
        var newExternalIdentifier = Guid.NewGuid().ToString();
        var mediaToSubscribeTo = Given.A.Media.MediaList.First();
        var subscribeMediaCommand = new SubscribeMediaCommand(newExternalIdentifier, mediaToSubscribeTo.Name);

        var subscribeMediaResult = await When.TheApplication.ReceivesCommand<SubscribeMediaCommand, Result>(subscribeMediaCommand);
        Then.TheResult(subscribeMediaResult).IsSuccessful();

        var mediaSubscriptions =
            await Then.TheApplication.ReceivesQuery<MediaSubscriptionsQuery, MediaSubscriptions>(new MediaSubscriptionsQuery(newExternalIdentifier));
        mediaSubscriptions.SubscribedToMediaNames.Should().Contain(mediaToSubscribeTo.Name);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Subscribing_fails_when_no_media_with_mediaName_exists()
    {
        await Given.TheDatabase.IsSeeded();
        var subscriber = Given.A.Subscriber.WithoutSubscriptions;
        var subscribeMediaCommand = new SubscribeMediaCommand(subscriber.ExternalIdentifier, "non existent");

        var subscribeMediaResult = await When.TheApplication.ReceivesCommand<SubscribeMediaCommand, Result>(subscribeMediaCommand);

        Then.TheResult(subscribeMediaResult).IsAFailure();
        Then.TheResult(subscribeMediaResult).ContainsError(Errors.General.NotFoundErrorCode);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Subscriber_is_not_created_when_media_with_mediaName_does_not_exist()
    {
        await Given.TheDatabase.IsSeeded();
        var newExternalIdentifier = "non existent";
        var subscribeMediaCommand = new SubscribeMediaCommand(newExternalIdentifier, "non existent");

        var subscribeMediaResult = await When.TheApplication.ReceivesCommand<SubscribeMediaCommand, Result>(subscribeMediaCommand);
        
        Then.TheResult(subscribeMediaResult).IsAFailure();
        Then.TheResult(subscribeMediaResult).ContainsError(Errors.General.NotFoundErrorCode);
        
        var mediaSubscriptions = await Then.TheApplication.ReceivesQuery<MediaSubscriptionsQuery, MediaSubscriptions>(new MediaSubscriptionsQuery(newExternalIdentifier));
        mediaSubscriptions.SubscribedToMediaNames.Should().HaveCount(0);
        
         (await Then.TheDatabase.GetSubscriberForExternalIdentifier("non existent"))
            .Should()
            .BeNull();
    }
}