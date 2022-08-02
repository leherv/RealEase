using FluentAssertions;
using ReleaseNotifierApp.Pages.Pagination;
using Xunit;

namespace ReleaseNotifierApp.Tests;

public class PaginationNavigationTests
{
    [Theory]
    [InlineData(1, 10, 0)]
    public void Only_one_NavigationNumber_visible(int pageIndex, int pageSize, int totalResultCount)
    {
        var navigationResult = PaginationNavigation.Build(pageIndex, pageSize, totalResultCount);
        navigationResult.First.Visible.Should().BeTrue();
        navigationResult.Second.Visible.Should().BeFalse();
        navigationResult.Third.Visible.Should().BeFalse();
    }
    
    [Theory]
    [InlineData(2, 5, 6)]
    public void Only_two_NavigationNumbers_visible(int pageIndex, int pageSize, int totalResultCount)
    {
        var navigationResult = PaginationNavigation.Build(pageIndex, pageSize, totalResultCount);
        navigationResult.First.Visible.Should().BeFalse();
        navigationResult.Second.Visible.Should().BeTrue();
        navigationResult.Third.Visible.Should().BeTrue();
    }
    
    [Theory]
    [InlineData(1, 10, 30)]
    [InlineData(1, 50, 1000)]
    [InlineData(3, 50, 300)]
    public void All_NavigationNumbers_visible_if_enough_items(int pageIndex, int pageSize, int totalResultCount)
    {
        var navigationResult = PaginationNavigation.Build(pageIndex, pageSize, totalResultCount);
        navigationResult.First.Visible.Should().BeTrue();
        navigationResult.Second.Visible.Should().BeTrue();
        navigationResult.Third.Visible.Should().BeTrue();
    }

    [Theory]
    [InlineData(1, 10, 30)]
    public void Correct_value_set_for_NavigationNumbers_if_PageIndex_at_beginning(int pageIndex, int pageSize, int totalResultCount)
    {
        var navigationResult = PaginationNavigation.Build(pageIndex, pageSize, totalResultCount);
        navigationResult.First.Visible.Should().BeTrue();
        navigationResult.First.Value.Should().Be(pageIndex);
        navigationResult.First.Active.Should().BeTrue();
        
        navigationResult.Second.Visible.Should().BeTrue();
        navigationResult.Second.Value.Should().Be(pageIndex + 1);
        
        navigationResult.Third.Visible.Should().BeTrue();
        navigationResult.Third.Value.Should().Be(pageIndex + 2);
    }
    
    [Theory]
    [InlineData(2, 10, 30)]
    [InlineData(5, 10, 51)]
    public void Correct_value_set_for_NavigationNumbers_if_PageIndex_in_the_middle(int pageIndex, int pageSize, int totalResultCount)
    {
        var navigationResult = PaginationNavigation.Build(pageIndex, pageSize, totalResultCount);
        navigationResult.First.Visible.Should().BeTrue();
        navigationResult.First.Value.Should().Be(pageIndex - 1);
        
        navigationResult.Second.Visible.Should().BeTrue();
        navigationResult.Second.Value.Should().Be(pageIndex);
        navigationResult.Second.Active.Should().BeTrue();
        
        navigationResult.Third.Visible.Should().BeTrue();
        navigationResult.Third.Value.Should().Be(pageIndex + 1);
    }
    
    [Theory]
    [InlineData(3, 10, 30)]
    public void Correct_value_set_for_NavigationNumbers_if_PageIndex_at_the_end(int pageIndex, int pageSize, int totalResultCount)
    {
        var navigationResult = PaginationNavigation.Build(pageIndex, pageSize, totalResultCount);
        navigationResult.First.Visible.Should().BeTrue();
        navigationResult.First.Value.Should().Be(pageIndex - 2);
        
        navigationResult.Second.Visible.Should().BeTrue();
        navigationResult.Second.Value.Should().Be(pageIndex - 1);
        
        navigationResult.Third.Visible.Should().BeTrue();
        navigationResult.Third.Value.Should().Be(pageIndex);
        navigationResult.Third.Active.Should().BeTrue();
    }
    
    
    
    [Theory]
    [InlineData(1, 10, 100)]
    [InlineData(1, 10, 0)]
    [InlineData(1, 5, 4)]
    public void Previous_should_be_disabled_on_first_page(int pageIndex, int pageSize, int totalResultCount)
    {
        var navigationResult = PaginationNavigation.Build(pageIndex, pageSize, totalResultCount);
        navigationResult.PreviousPossible.Should().BeFalse();
    }
    
    [Theory]
    [InlineData(2, 10, 100)]
    [InlineData(2, 4, 5)]
    public void Previous_should_be_enabled_if_not_on_first_page(int pageIndex, int pageSize, int totalResultCount)
    {
        var navigationResult = PaginationNavigation.Build(pageIndex, pageSize, totalResultCount);
        navigationResult.PreviousPossible.Should().BeTrue();
    }

    [Theory]
    [InlineData(1, 10, 0)]
    [InlineData(1, 10, 10)]
    [InlineData(2, 10, 20)]
    public void Next_should_be_disabled_if_not_enough_items(int pageIndex, int pageSize, int totalResultCount)
    {
        var navigationResult = PaginationNavigation.Build(pageIndex, pageSize, totalResultCount);
        navigationResult.NextPossible.Should().BeFalse();
    }
    
    [Theory]
    [InlineData(1, 10, 20)]
    [InlineData(1, 10, 11)]
    [InlineData(2, 100, 1000)]
    public void Next_should_be_enabled_if_enough_items(int pageIndex, int pageSize, int totalResultCount)
    {
        var navigationResult = PaginationNavigation.Build(pageIndex, pageSize, totalResultCount);
        navigationResult.NextPossible.Should().BeTrue();
    }
}