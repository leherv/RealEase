using System.Collections.Generic;
using Domain.Model;
using FluentAssertions;
using Xunit;

namespace Domain.Tests;

public class ReleaseNumberTests
{
    private static IEnumerable<object[]> ReleaseNumbers() => new List<object[]>
    {
        new object[] { 1, 1, 1, 0, true},
        new object[] { 2, 1, 1, 2, true},
        new object[] { 2, 1, 1, 2, true },
        new object[] { 1, 0, 1, 1, false },
        new object[] { 230, 9, 231, 0, false },
        new object[] { 1, 0, 1, 0, false },
        new object[] { 1, 1, 1, 1, false },
        new object[] { 230, 9, 230, 9, false }
    };
        
    [Theory]
    [MemberData(nameof(ReleaseNumbers))]
    public void Determines_if_ReleaseNumber_is_newer_than_other_ReleaseNumber(int major, int minor, int majorOther, int minorOther, bool expected)
    {
        var releaseNumber = ReleaseNumber.Create(major, minor);
        var releaseNumberOther = ReleaseNumber.Create(majorOther, minorOther);

        var isOtherNewer = releaseNumber.Value.IsNewerThan(releaseNumberOther.Value);

        isOtherNewer.Should().Be(expected);
    }
}