using FluentAssertions;
using RealEaseApp.Pages.Pagination;
using Xunit;

namespace RealEaseApp.Tests;

public class PaginationNavigationTests
{
    [Theory]
    [InlineData(1, 10, 0, "(1)")]
    [InlineData(2, 5, 6, "1 (2)")]
    [InlineData(1, 10, 30, "(1) 2 3")]
    [InlineData(1, 50, 1000, "(1) 2 3 20")]
    [InlineData(3, 50, 300, "1 2 (3) 4 6")]
    [InlineData(2, 10, 30, "1 (2) 3")]
    [InlineData(5, 10, 51, "1 4 (5) 6")]
    [InlineData(3, 10, 30, "1 2 (3)")]
    [InlineData(400, 10, 30, "1 2 (3)")]
    [InlineData(-1, 10, 30, "(1) 2 3")]
    public void Builds_expected_NavigationPages(int pageIndex, int pageSize, int totalResultCount, string expected)
    {
        var paginationNavigationBuilder = new PaginationNavigationBuilder(pageIndex, pageSize, totalResultCount);
        var navigationResult = paginationNavigationBuilder.Build();
        
        navigationResult.ToDisplayString().Should().Be(expected);
    }
    
    [Theory]
    [InlineData(1, 10, 100)]
    [InlineData(1, 10, 0)]
    [InlineData(1, 5, 4)]
    public void Previous_should_be_disabled_on_first_page(int pageIndex, int pageSize, int totalResultCount)
    {
        var paginationNavigationBuilder = new PaginationNavigationBuilder(pageIndex, pageSize, totalResultCount);
        var navigationResult = paginationNavigationBuilder.Build();
        
        navigationResult.PreviousPossible.Should().BeFalse();
    }
    
    [Theory]
    [InlineData(2, 10, 100)]
    [InlineData(2, 4, 5)]
    public void Previous_should_be_enabled_if_not_on_first_page(int pageIndex, int pageSize, int totalResultCount)
    {
        var paginationNavigationBuilder = new PaginationNavigationBuilder(pageIndex, pageSize, totalResultCount);
        var navigationResult = paginationNavigationBuilder.Build();
        
        navigationResult.PreviousPossible.Should().BeTrue();
    }

    [Theory]
    [InlineData(1, 10, 0)]
    [InlineData(1, 10, 10)]
    [InlineData(2, 10, 20)]
    [InlineData(444, 10, 20)]
    public void Next_should_be_disabled_if_not_enough_items(int pageIndex, int pageSize, int totalResultCount)
    {
        var paginationNavigationBuilder = new PaginationNavigationBuilder(pageIndex, pageSize, totalResultCount);
        
        var navigationResult = paginationNavigationBuilder.Build();
        navigationResult.NextPossible.Should().BeFalse();
    }
    
    [Theory]
    [InlineData(1, 10, 20)]
    [InlineData(1, 10, 11)]
    [InlineData(2, 100, 1000)]
    public void Next_should_be_enabled_if_enough_items(int pageIndex, int pageSize, int totalResultCount)
    {
        var paginationNavigationBuilder = new PaginationNavigationBuilder(pageIndex, pageSize, totalResultCount);
        
        var navigationResult = paginationNavigationBuilder.Build();
        navigationResult.NextPossible.Should().BeTrue();
    }
}