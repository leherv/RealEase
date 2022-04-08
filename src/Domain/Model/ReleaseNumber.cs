using Domain.Invariants;
using Domain.Invariants.Extensions;
using Domain.Results;

namespace Domain.Model;

public record ReleaseNumber
{
    public int Major { get; }
    public int Minor { get; }

    private ReleaseNumber(int major, int minor)
    {
        Major = major;
        Minor = minor;
    }

    public static Result<ReleaseNumber> Create(int major, int minor)
    {
        return Invariant.Create
            .GreaterThanOrEqualTo(0, major, nameof(major))
            .GreaterThanOrEqualTo(0, minor, nameof(minor))
            .ValidateAndCreate(() => new ReleaseNumber(major, minor));
    }

    public bool IsNewerOrEqualTo(ReleaseNumber other)
    {
        if (Major > other.Major)
            return true;

        return Major == other.Major && Minor >= other.Minor;
    }
}