using Domain.Model;
using Domain.Results;

namespace Shared;

public class GivenTheMedia
{
    public List<Media> MediaList { get; }
    public Media WithoutSubscribersAndReleases { get; }
    public Media WithSubscriberWithoutReleases { get; }
    public Media WithSubscriberWithRelease { get; }
    public Release CurrentRelease { get; }
    public Media WithSubscriberWithoutRelease { get; }

    public GivenTheMedia()
    {
        MediaList = new List<Media>
        {
            Create(Guid.NewGuid(), "Hunter x Hunter").Value,
            Create(Guid.NewGuid(), "Solo Leveling").Value,
            Create(Guid.NewGuid(), "Bleach").Value,
            Create(Guid.NewGuid(), "Naruto").Value
        };

        WithSubscriberWithoutRelease = MediaList.First();
        WithSubscriberWithoutReleases = MediaList.Skip(1).First();
        
        
        WithSubscriberWithRelease = MediaList.Skip(2).First();
        CurrentRelease = GivenTheRelease.Create(
            ReleaseNumber.Create(3, 0).Value, "https://www.thisIsATest.com/chapter/3"
        ).Value;
        WithSubscriberWithRelease.PublishNewRelease(CurrentRelease);
        WithoutSubscribersAndReleases = MediaList.Skip(3).First();
    }

    public static Result<Media> Create(
        Guid? id = null,
        string? mediaName = null
    )
    {
        return Media.Create(
            id ?? Guid.NewGuid(),
            mediaName ?? "Hunter x Hunter"
        );
    }
}