using FluentAssertions;
using Infrastructure.Scraper;
using Infrastructure.Scraper.Shared;
using Xunit;

namespace Intrastructure.Scraper.Tests;

public class UriCombinatorTests
{
    [Theory]
    [InlineData("https://earlymng.org/", "/martial-peak/chapter-2178", "https://earlymng.org/martial-peak/chapter-2178")]
    [InlineData("https://earlymng.org", "/martial-peak/chapter-2178", "https://earlymng.org/martial-peak/chapter-2178")]
    [InlineData("https://earlymng.org/", "martial-peak/chapter-2178", "https://earlymng.org/martial-peak/chapter-2178")]
    [InlineData("https://earlymng.org", "martial-peak/chapter-2178", "https://earlymng.org/martial-peak/chapter-2178")]
    public void Test1(string uri, string relativeUri, string expectedUri)
    {
        var combinedUri = UriCombinator.Combine(uri, relativeUri);
        combinedUri.Should().Be(expectedUri);
    }
}