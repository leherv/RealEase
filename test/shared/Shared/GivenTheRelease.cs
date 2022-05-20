using Domain.Model;
using Domain.Results;

namespace Shared;

public class GivenTheRelease
{
    public static Result<Release> Create(
        ReleaseNumber? releaseNumber = null,
        ResourceUrl? resourceUrl = null,
        DateTime? datetime = null
    )
    {
        return Release.Create(
            releaseNumber ?? ReleaseNumber.Create(1, 0).Value,
            resourceUrl ?? ResourceUrl.Create("https://www.thisisatest.com/test/chapter/1").Value,
            DateTime.UtcNow
        );
    }
}