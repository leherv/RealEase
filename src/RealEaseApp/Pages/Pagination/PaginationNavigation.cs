namespace RealEaseApp.Pages.Pagination;

public record PaginationNavigation(
    bool PreviousPossible,
    IReadOnlyCollection<NavigationItem> NavigationItems,
    bool NextPossible
)
{
    public string ToDisplayString()
    {
        return NavigationItems
            .Where(navigationItem => navigationItem.Visible)
            .Aggregate("", (current, navigationPage) => current + (string.IsNullOrWhiteSpace(current)
                ? PrintValue(navigationPage)
                : $" {PrintValue(navigationPage)}"));
    }

    public int ActivePageIndex => NavigationItems
        .Where(navigationItem => navigationItem.Active)
        .Select(navigationItem => navigationItem.Value)
        .Single();

    private static string PrintValue(NavigationItem navigationItem)
    {
        return navigationItem.Active
            ? $"({navigationItem.Value})"
            : navigationItem.Value.ToString();
    }
}