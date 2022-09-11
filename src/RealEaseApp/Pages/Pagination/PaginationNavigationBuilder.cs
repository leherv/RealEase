namespace RealEaseApp.Pages.Pagination;

internal class PaginationNavigationBuilder
{
    private readonly int _pageIndex;
    private readonly int _pageSize;
    private readonly int _totalResultCount;
    private readonly int _totalPageCount;

    internal PaginationNavigationBuilder(int pageIndex, int pageSize, int totalResultCount)
    {
        _pageSize = pageSize;
        _totalResultCount = totalResultCount;
        _totalPageCount = TotalPageCount();
        _pageIndex = GetAdjustedPageIndex(pageIndex);
    }

    internal PaginationNavigation Build()
    {
        var paginationNavigationItems = new List<NavigationItem>();

        if (!StartPageInRange())
        {
            paginationNavigationItems.Add(new NavigationItem(1, true, false));
        }

        if (StartIsActive())
        {
            paginationNavigationItems.Add(new NavigationItem(_pageIndex, true, true));
            paginationNavigationItems.Add(new NavigationItem(_pageIndex + 1, _totalPageCount > 1, false));
            paginationNavigationItems.Add(new NavigationItem(_pageIndex + 2, _totalPageCount > 2, false));
        }
        else if (EndIsActive())
        {
            paginationNavigationItems.Add(new NavigationItem(_pageIndex - 2, _pageIndex - 2 > 0, false));
            paginationNavigationItems.Add(new NavigationItem(_pageIndex - 1, _pageIndex - 1 > 0, false));
            paginationNavigationItems.Add(new NavigationItem(_pageIndex, true, true));
        }
        else if (_pageIndex > 1)
        {
            paginationNavigationItems.Add(new NavigationItem(_pageIndex - 1, true, false));
            paginationNavigationItems.Add(new NavigationItem(_pageIndex, true, true));
            paginationNavigationItems.Add(new NavigationItem(_pageIndex + 1, _totalPageCount > _pageIndex, false));
        }

        if (!EndPageInRange())
        {
            paginationNavigationItems.Add(new NavigationItem(_totalPageCount, true, false));
        }

        return new PaginationNavigation(
            PreviousActive(),
            paginationNavigationItems,
            NextActive()
        );
    }

    private int GetAdjustedPageIndex(int pageIndex)
    {
        if (pageIndex <= 1)
            return 1;
        return pageIndex > _totalPageCount ? 
            _totalPageCount : 
            pageIndex;
    }

    private bool StartPageInRange()
    {
        return _pageIndex - 2 <= 1 && EndIsActive() ||
               StartIsActive() ||
               _pageIndex - 1 == 1;
    }

    private bool EndPageInRange()
    {
        return _pageIndex + 2 >= _totalPageCount;
    }

    private bool EndIsActive()
    {
        return _pageIndex >= _totalPageCount;
    }

    private bool StartIsActive()
    {
        return _pageIndex == 1;
    }

    private bool NextActive()
    {
        return _totalPageCount > _pageIndex;
    }

    private bool PreviousActive()
    {
        return _pageIndex > 1;
    }

    private int TotalPageCount()
    {
        return (int)Math.Ceiling((double)_totalResultCount / _pageSize);
    }
}