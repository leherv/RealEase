using Application.Ports.Persistence.Read;
using FluentAssertions;
using Xunit;

namespace Application.Tests;

public class QueryParameterTests
{
    [Theory]
    [InlineData(1, 50, 0)]
    [InlineData(2, 50, 50)]
    [InlineData(1, 1, 0)]
    [InlineData(2, 1, 1)]
    public void Calculates_skip_matching_PageIndex_and_PageSize(int PageIndex, int PageSize, int expectedSkip)
    {
        var sut = new QueryParameters(PageIndex, PageSize);
        var actualSkip = sut.CalculateSkipForQuery();
        actualSkip.Should().Be(expectedSkip);
    }
}