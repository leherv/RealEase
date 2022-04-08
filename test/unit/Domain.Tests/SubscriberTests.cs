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

        subscriber.Subscribe(media);

        subscriber.SubscribedToMedia.Should().HaveCount(1);
        subscriber.SubscribedToMedia.Should().Contain(media);
    }

    [Fact]
    public void Subscribe_succeeds_if_already_subscribed()
    {
        var subscriber = GivenTheSubscriber.Create().Value;
        var media = GivenTheMedia.Create().Value;
        subscriber.Subscribe(media);
        
        subscriber.Subscribe(media);

        subscriber.SubscribedToMedia.Should().HaveCount(1);
        subscriber.SubscribedToMedia.Should().Contain(media);
    }

    [Fact]
    public void Unsubscribe_succeeds_if_subscribed_to_media()
    {
        var subscriber = GivenTheSubscriber.Create().Value;
        var media = GivenTheMedia.Create().Value;
        subscriber.Subscribe(media);

        var result = subscriber.Unsubscribe(media);

        result.IsSuccess.Should().BeTrue();
        subscriber.SubscribedToMedia.Should().NotContain(media);
        subscriber.SubscribedToMedia.Should().BeEmpty();
    }
    
    [Fact]
    public void Unsubscribe_succeeds_if_no_subscriptions_yet()
    {
        var subscriber = GivenTheSubscriber.Create().Value;
        var media = GivenTheMedia.Create().Value;

        var result = subscriber.Unsubscribe(media);

        result.IsSuccess.Should().BeTrue();
        subscriber.SubscribedToMedia.Should().NotContain(media);
        subscriber.SubscribedToMedia.Should().BeEmpty();
    }
    
    [Fact]
    public void Unsubscribe_succeeds_if_not_subscribed_to_this_media()
    {
        var subscriber = GivenTheSubscriber.Create().Value;
        var media = GivenTheMedia.Create().Value;
        subscriber.Subscribe(media);
        var otherMedia = GivenTheMedia.Create(mediaName: "Other media").Value;

        var result = subscriber.Unsubscribe(otherMedia);

        result.IsSuccess.Should().BeTrue();
        subscriber.SubscribedToMedia.Should().NotContain(otherMedia);
        subscriber.SubscribedToMedia.Should().HaveCount(1);
    }
}