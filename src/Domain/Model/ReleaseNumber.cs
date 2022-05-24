using Domain.Invariants;
using Domain.Invariants.Extensions;
using Domain.Results;

namespace Domain.Model;

public record ReleaseNumber : IComparable<ReleaseNumber>
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

    public bool IsNewerThan(ReleaseNumber other)
    {
        if (Major > other.Major)
            return true;

        return Major == other.Major && Minor > other.Minor;
    }

    public int CompareTo(ReleaseNumber? other)
    {
        if (other == null)
            return 1;
        
        if (Equals(this))
            return 0;
        
        if (Major > other.Major)
            return 1;

        return Major == other.Major && Minor > other.Minor
            ? 1
            : -1;
    }
}