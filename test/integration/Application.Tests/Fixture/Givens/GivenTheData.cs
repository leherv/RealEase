
using Shared;

namespace Application.Test.Fixture.Givens;

public class GivenTheData
{
    public GivenTheMedia Media { get; }
    public GivenTheSubscriber Subscriber { get; }

    public GivenTheData()
    {
        Media = new GivenTheMedia();
        Subscriber = new GivenTheSubscriber(Media);
    }
}