using FluentAssertions;
using Infrastructure.Scraper.Shared;
using Xunit;

namespace Infrastructure.Scraper.Tests;

public class UriReleaseNumberExtractorTests
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
    [InlineData("https://mangapill.com/chapters/5134-10149000/the-beginning-after-the-end-chapter-149", 149, 0 )]
    [InlineData("https://mangapill.com/chapters/5134-10001000/the-beginning-after-the-end-chapter-1", 1, 0 )]
    [InlineData("https://mangapill.com/chapters/4270-10382500/tales-of-demons-and-gods-chapter-382.5", 382, 5 )]
    [InlineData("https://mangapill.com/chapters/4270-10300100/tales-of-demons-and-gods-chapter-300.1", 300, 1 )]
    [InlineData("https://mangapill.com/chapters/4270-10128199/tales-of-demons-and-gods-chapter-128.199", 128, 199 )]
    public void Test1(string chapterUrl, int expectedMajor, int expectedMinor)
    {
        var result = UriReleaseNumberExtractor.ExtractReleaseNumbers(chapterUrl);
        result.IsSuccess.Should().BeTrue();
        result.Value.Major.Should().Be(expectedMajor);
        result.Value.Minor.Should().Be(expectedMinor);
    }
}