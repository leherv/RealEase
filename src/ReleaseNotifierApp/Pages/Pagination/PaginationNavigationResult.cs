namespace ReleaseNotifierApp.Pages.Pagination;

public record PaginationNavigationResult(
    bool PreviousPossible,
    IReadOnlyCollection<NavigationNumber> NavigationNumbers,
    bool NextPossible
)
{
    public NavigationNumber First => NavigationNumbers.ElementAt(0);
    public NavigationNumber Second => NavigationNumbers.ElementAt(1);
    public NavigationNumber Third => NavigationNumbers.ElementAt(2);
};