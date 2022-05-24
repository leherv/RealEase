using FluentAssertions;
using Infrastructure.Scraper.Shared;
using Xunit;

namespace Infrastructure.Scraper.Tests;

public class ReleaseNumberExtractorTests
{
    [Theory]
    [InlineData("Episode 1", 1, 0 )]
    [InlineData("Episode 0", 0, 0 )]
    public void Test1(string value, int expectedMajor, int expectedMinor)
    {
        var result = ReleaseNumberExtractor.ExtractReleaseNumbers(value);
        result.IsSuccess.Should().BeTrue();
        result.Value.Major.Should().Be(expectedMajor);
        result.Value.Minor.Should().Be(expectedMinor);
    }
}