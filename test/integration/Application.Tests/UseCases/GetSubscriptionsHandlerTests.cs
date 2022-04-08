using System.Threading.Tasks;
using Application.Test.Fixture;
using Application.UseCases.Subscriber.QueryMediaSubscriptions;
using FluentAssertions;
using Xunit;

namespace Application.Test.UseCases;

public class GetSubscriptionsHandlerTests : IntegrationTestBase
{
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Returns_all_subscriptions_for_Subscriber_with_subscriptions()
    {
        await Given.TheDatabase.IsSeeded();
        var subscriberWithSubscriptions = Given.A.Subscriber.WithSubscriptions;
        var mediaSubscriptionsQuery = new MediaSubscriptionsQuery(subscriberWithSubscriptions.ExternalIdentifier);

        var mediaSubscriptions =
            await When.TheApplication.ReceivesQuery<MediaSubscriptionsQuery, MediaSubscriptions>(
                mediaSubscriptionsQuery);

        mediaSubscriptions.SubscribedToMediaNames
            .Should()
            .HaveCount(Given.A.Subscriber.WithSubscriptions.Subscriptions.Count);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Returns_empty_collection_if_no_subscriptions_yet()
    {
        await Given.TheDatabase.IsSeeded();
        var subscriberWithOutSubscriptions = Given.A.Subscriber.WithoutSubscriptions;
        var mediaSubscriptionsQuery = new MediaSubscriptionsQuery(subscriberWithOutSubscriptions.ExternalIdentifier);

        var mediaSubscriptions =
            await When.TheApplication.ReceivesQuery<MediaSubscriptionsQuery, MediaSubscriptions>(
                mediaSubscriptionsQuery);

        mediaSubscriptions.SubscribedToMediaNames
            .Should()
            .HaveCount(0);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Returns_empty_list_if_no_subscriber_with_ExternalIdentifier_not_found()
    {
        await Given.TheDatabase.IsSeeded();
        var mediaSubscriptionsQuery = new MediaSubscriptionsQuery("non existent");

        var mediaSubscriptions =
            await When.TheApplication.ReceivesQuery<MediaSubscriptionsQuery, MediaSubscriptions>(
                mediaSubscriptionsQuery);

        mediaSubscriptions.SubscribedToMediaNames
            .Should()
            .HaveCount(0);
    }
}