namespace RealEaseApp.Pages.Pagination;

public static class PaginationNavigationBuilder
{
    public static PaginationNavigation Build(int pageIndex, int pageSize, int totalResultCount)
    {
        var totalPageCount = TotalPageCount(totalResultCount, pageSize);
        var paginationNavigationItems = new List<NavigationItem>();

        if (!StartPageInRange(pageIndex, totalPageCount))
        {
            paginationNavigationItems.Add(new NavigationItem(1, true, false));
        }

        if (StartIsActive(pageIndex))
        {
            paginationNavigationItems.Add(new NavigationItem(pageIndex, true, true));
            paginationNavigationItems.Add(new NavigationItem(pageIndex + 1, totalPageCount > 1, false));
            paginationNavigationItems.Add(new NavigationItem(pageIndex + 2, totalPageCount > 2, false));
        }
        else if (EndIsActive(pageIndex, totalPageCount))
        {
            paginationNavigationItems.Add(new NavigationItem(pageIndex - 2, pageIndex - 2 > 0, false));
            paginationNavigationItems.Add(new NavigationItem(pageIndex - 1, pageIndex - 1 > 0, false));
            paginationNavigationItems.Add(new NavigationItem(pageIndex, true, true));
        }
        else if (pageIndex > 1)
        {
            paginationNavigationItems.Add(new NavigationItem(pageIndex - 1, true, false));
            paginationNavigationItems.Add(new NavigationItem(pageIndex, true, true));
            paginationNavigationItems.Add(new NavigationItem(pageIndex + 1, totalPageCount > pageIndex, false));
        }

        if (!EndPageInRange(pageIndex, totalPageCount))
        {
            paginationNavigationItems.Add(new NavigationItem(totalPageCount, true, false));
        }

        return new PaginationNavigation(
            PreviousActive(pageIndex),
            paginationNavigationItems,
            NextActive(pageIndex, totalPageCount)
        );
    }

    private static bool StartPageInRange(int pageIndex, int totalPageCount)
    {
        return pageIndex - 2 <= 1 && EndIsActive(pageIndex, totalPageCount) ||
               StartIsActive(pageIndex) ||
               pageIndex - 1 == 1;
    }

    private static bool EndPageInRange(int pageIndex, int totalPageCount)
    {
        return pageIndex + 2 >= totalPageCount;
    }

    private static bool EndIsActive(int pageIndex, int totalPageCount)
    {
        return pageIndex >= totalPageCount;
    }

    private static bool StartIsActive(int pageIndex)
    {
        return pageIndex == 1;
    }

    private static bool NextActive(int pageIndex, int totalPageCount)
    {
        return totalPageCount > pageIndex;
    }

    private static bool PreviousActive(int pageIndex)
    {
        return pageIndex > 1;
    }

    private static int TotalPageCount(int totalResultCount, int pageSize)
    {
        return (int)Math.Ceiling((double)totalResultCount / pageSize);
    }
}