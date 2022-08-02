namespace ReleaseNotifierApp.Pages.Pagination;

public static class PaginationNavigation
{
    public static PaginationNavigationResult Build(int pageIndex, int pageSize, int totalResultCount)
    {
        var previousActive = pageIndex > 1;
        var pageCount = (double)totalResultCount / pageSize;
        var nextActive = pageCount > pageIndex;

        var navigationNumbers = new List<NavigationNumber>();
        
        if (pageIndex == 1)
        {
            navigationNumbers.Add(new NavigationNumber(pageIndex, true, true));
            navigationNumbers.Add(new NavigationNumber(pageIndex + 1, pageCount > 1, false));
            navigationNumbers.Add(new NavigationNumber(pageIndex + 2, pageCount > 2, false));
        } else if (pageIndex >= pageCount)
        {
            navigationNumbers.Add(new NavigationNumber(pageIndex - 2, pageIndex - 2 > 0, false));
            navigationNumbers.Add(new NavigationNumber(pageIndex - 1, pageIndex - 1 > 0, false));
            navigationNumbers.Add(new NavigationNumber(pageIndex, true, true));
        } 
        else if (pageIndex > 1)
        {
            navigationNumbers.Add(new NavigationNumber(pageIndex - 1, true, false));
            navigationNumbers.Add(new NavigationNumber(pageIndex, true, true));
            navigationNumbers.Add(new NavigationNumber(pageIndex + 1, pageCount > pageIndex, false));
        }
        
        return new PaginationNavigationResult(
            previousActive,
            navigationNumbers,
            nextActive
        );
    }
}