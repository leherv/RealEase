using Domain.Model;
using Domain.Results;

namespace Shared;

public class GivenTheRelease
{
    public static Result<Release> Create(
        ReleaseNumber? releaseNumber = null,
        string? link = null
    )
    {
        return Release.Create(
            releaseNumber ?? ReleaseNumber.Create(1, 0).Value,
            "https://www.thisisatest.com/test/chapter/1"
        );
    }
}