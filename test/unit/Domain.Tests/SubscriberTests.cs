using FluentAssertions;
using Shared;
using Xunit;

namespace Domain.Tests;

public class SubscriberTests
{
    [Fact]
    public void Subscribe_succeeds_if_not_subscribed_yet()
    {
        var subscriber = GivenTheSubscriber.Create().Value;
        var media = GivenTheMedia.Create().Value;

        subscriber.Subscribe(media.Id);

        subscriber.SubscribedToMediaIds.Should().HaveCount(1);
        subscriber.SubscribedToMediaIds.Should().Contain(media.Id);
    }

    [Fact]
    public void Subscribe_succeeds_if_already_subscribed()
    {
        var subscriber = GivenTheSubscriber.Create().Value;
        var media = GivenTheMedia.Create().Value;
        subscriber.Subscribe(media.Id);
        
        subscriber.Subscribe(media.Id);

        subscriber.SubscribedToMediaIds.Should().HaveCount(1);
        subscriber.SubscribedToMediaIds.Should().Contain(media.Id);
    }

    [Fact]
    public void Unsubscribe_succeeds_if_subscribed_to_media()
    {
        var subscriber = GivenTheSubscriber.Create().Value;
        var media = GivenTheMedia.Create().Value;
        subscriber.Subscribe(media.Id);

        var result = subscriber.Unsubscribe(media.Id);

        result.IsSuccess.Should().BeTrue();
        subscriber.SubscribedToMediaIds.Should().NotContain(media.Id);
        subscriber.SubscribedToMediaIds.Should().BeEmpty();
    }
    
    [Fact]
    public void Unsubscribe_succeeds_if_no_subscriptions_yet()
    {
        var subscriber = GivenTheSubscriber.Create().Value;
        var media = GivenTheMedia.Create().Value;

        var result = subscriber.Unsubscribe(media.Id);

        result.IsSuccess.Should().BeTrue();
        subscriber.SubscribedToMediaIds.Should().NotContain(media.Id);
        subscriber.SubscribedToMediaIds.Should().BeEmpty();
    }
    
    [Fact]
    public void Unsubscribe_succeeds_if_not_subscribed_to_this_media()
    {
        var subscriber = GivenTheSubscriber.Create().Value;
        var media = GivenTheMedia.Create().Value;
        subscriber.Subscribe(media.Id);
        var otherMedia = GivenTheMedia.Create(mediaName: "Other media").Value;

        var result = subscriber.Unsubscribe(otherMedia.Id);

        result.IsSuccess.Should().BeTrue();
        subscriber.SubscribedToMediaIds.Should().NotContain(otherMedia.Id);
        subscriber.SubscribedToMediaIds.Should().HaveCount(1);
    }
}