using FluentAssertions;
using Infrastructure.Scraper;
using Xunit;

namespace Intrastructure.Scraper.Tests;

public class ReleaseNumberExtractorTests
{
    [Theory]
    [InlineData("/manga/martial-peak/chapter-2178", 2178, 0 )]
    [InlineData("/manga/325643546/chapter-377-1", 377, 1 )]
    [InlineData("/manga/seoul-station-s-necromancer/chapter-43", 43, 0 )]
    [InlineData("/manga/hoarding-in-hell/chapter-31", 31, 0 )]
    [InlineData("/manga/325643546/chapter-376-6", 376, 6 )]
    public void Test1(string chapterUrl, int expectedMajor, int expectedMinor)
    {
        var result = ReleaseNumberExtractor.ExtractReleaseNumbers(chapterUrl);
        result.IsSuccess.Should().BeTrue();
        result.Value.Major.Should().Be(expectedMajor);
        result.Value.Minor.Should().Be(expectedMinor);
    }
}