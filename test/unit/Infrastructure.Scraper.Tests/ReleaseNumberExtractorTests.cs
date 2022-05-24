using FluentAssertions;
using Infrastructure.Scraper;
using Infrastructure.Scraper.Shared;
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
    [InlineData("https://readmanganato.com/manga-bn978870/chapter-2255", 2255, 0 )]
    [InlineData("https://readmanganato.com/manga-ax951880/chapter-380.1", 380, 1 )]
    [InlineData("https://readmanganato.com/manga-ax951880/chapter-378.6", 378, 6 )]
    [InlineData("https://readmanganato.com/manga-dr980474/chapter-179.2", 179, 2 )]
    [InlineData("https://readmanganato.com/manga-dr980474/chapter-0", 0, 0 )]
    public void Test1(string chapterUrl, int expectedMajor, int expectedMinor)
    {
        var result = ReleaseNumberExtractor.ExtractReleaseNumbers(chapterUrl);
        result.IsSuccess.Should().BeTrue();
        result.Value.Major.Should().Be(expectedMajor);
        result.Value.Minor.Should().Be(expectedMinor);
    }
}