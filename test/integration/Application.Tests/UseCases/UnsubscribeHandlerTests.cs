using System.Linq;
using System.Threading.Tasks;
using Application.Test.Fixture;
using Application.UseCases.Subscriber.QueryMediaSubscriptions;
using Application.UseCases.Subscriber.UnsubscribeMedia;
using Domain.ApplicationErrors;
using Domain.Results;
using FluentAssertions;
using Xunit;

namespace Application.Test.UseCases;

public class UnsubscribeHandlerTests : IntegrationTestBase
{
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Unsubscribing_from_subscribed_to_media_removes_it_from_subscriptions()
    {
        await Given.TheDatabase.IsSeeded();
        var subscriber = Given.A.Subscriber.WithSubscriptions;
        var mediaToUnsubscribeFrom = Given.A.Subscriber.SubscribedToMedia.First();
        var unsubscribeMediaCommand = new UnsubscribeMediaCommand(subscriber.ExternalIdentifier, mediaToUnsubscribeFrom.Name);

        var unSubscribeMediaResult = await When.TheApplication.ReceivesCommand<UnsubscribeMediaCommand, Result>(unsubscribeMediaCommand);

        Then.TheResult(unSubscribeMediaResult).IsSuccessful();
        var mediaSubscriptions =
            await Then.TheApplication.ReceivesQuery<MediaSubscriptionsQuery, MediaSubscriptions>(new MediaSubscriptionsQuery(subscriber.ExternalIdentifier));
        mediaSubscriptions.Should().NotBeNull();
        mediaSubscriptions.SubscribedToMedia.Should().HaveCount(subscriber.Subscriptions.Count - 1);
        mediaSubscriptions.SubscribedToMedia
            .Select(subscribedToMedia => subscribedToMedia.Name)
            .Should()
            .NotContain(mediaToUnsubscribeFrom.Name);
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Unsubscribing_from_media_succeeds_if_no_subscriptions_yet()
    {
        await Given.TheDatabase.IsSeeded();
        var subscriber = Given.A.Subscriber.WithoutSubscriptions;
        var mediaToUnsubscribeFrom = Given.A.Media.MediaList.First();
        var unsubscribeMediaCommand = new UnsubscribeMediaCommand(subscriber.ExternalIdentifier, mediaToUnsubscribeFrom.Name);

        var unSubscribeMediaResult = await When.TheApplication.ReceivesCommand<UnsubscribeMediaCommand, Result>(unsubscribeMediaCommand);

        Then.TheResult(unSubscribeMediaResult).IsSuccessful();
        var mediaSubscriptions =
            await Then.TheApplication.ReceivesQuery<MediaSubscriptionsQuery, MediaSubscriptions>(new MediaSubscriptionsQuery(subscriber.ExternalIdentifier));
        mediaSubscriptions.Should().NotBeNull();
        mediaSubscriptions.SubscribedToMedia.Should().BeEmpty();
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Unsubscribing_from_media_succeeds_if_not_subscribed_to_this_media()
    {
        await Given.TheDatabase.IsSeeded();
        var subscriber = Given.A.Subscriber.WithSubscriptions;
        var mediaToUnsubscribeFrom = Given.A.Subscriber.NotSubscribedToMedia.First();
        var unsubscribeMediaCommand = new UnsubscribeMediaCommand(subscriber.ExternalIdentifier, mediaToUnsubscribeFrom.Name);

        var unSubscribeMediaResult = await When.TheApplication.ReceivesCommand<UnsubscribeMediaCommand, Result>(unsubscribeMediaCommand);

        Then.TheResult(unSubscribeMediaResult).IsSuccessful();
        var mediaSubscriptions =
            await Then.TheApplication.ReceivesQuery<MediaSubscriptionsQuery, MediaSubscriptions>(new MediaSubscriptionsQuery(subscriber.ExternalIdentifier));
        mediaSubscriptions.Should().NotBeNull();
        mediaSubscriptions.SubscribedToMedia.Should().HaveCount(subscriber.Subscriptions.Count);
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Unsubscribing_from_non_existent_media_is_not_successful()
    {
        await Given.TheDatabase.IsSeeded();
        var subscriber = Given.A.Subscriber.WithSubscriptions;
        var unsubscribeMediaCommand = new UnsubscribeMediaCommand(subscriber.ExternalIdentifier, "non existent");

        var unSubscribeMediaResult = await When.TheApplication.ReceivesCommand<UnsubscribeMediaCommand, Result>(unsubscribeMediaCommand);

        Then.TheResult(unSubscribeMediaResult).IsAFailure();
        Then.TheResult(unSubscribeMediaResult).ContainsErrorWithCode(Errors.General.NotFoundErrorCode);
        var mediaSubscriptions =
            await Then.TheApplication.ReceivesQuery<MediaSubscriptionsQuery, MediaSubscriptions>(new MediaSubscriptionsQuery(subscriber.ExternalIdentifier));
        mediaSubscriptions.Should().NotBeNull();
        mediaSubscriptions.SubscribedToMedia.Should().HaveCount(subscriber.Subscriptions.Count);
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Unsubscribing_is_not_successful_if_no_Subscriber_is_found()
    {
        await Given.TheDatabase.IsSeeded();
        const string nonExistentSubscriberExternalId = "non existent";
        var mediaToUnsubscribeFrom = Given.A.Media.MediaList.First();
        var unsubscribeMediaCommand = new UnsubscribeMediaCommand(nonExistentSubscriberExternalId, mediaToUnsubscribeFrom.Name);

        var unSubscribeMediaResult = await When.TheApplication.ReceivesCommand<UnsubscribeMediaCommand, Result>(unsubscribeMediaCommand);

        Then.TheResult(unSubscribeMediaResult).IsAFailure();
        Then.TheResult(unSubscribeMediaResult).ContainsErrorWithCode(Errors.General.NotFoundErrorCode);
        var mediaSubscriptions =
            await Then.TheApplication.ReceivesQuery<MediaSubscriptionsQuery, MediaSubscriptions>(new MediaSubscriptionsQuery(nonExistentSubscriberExternalId));
        mediaSubscriptions.Should().NotBeNull();
        mediaSubscriptions.SubscribedToMedia.Should().HaveCount(0);
    }
}