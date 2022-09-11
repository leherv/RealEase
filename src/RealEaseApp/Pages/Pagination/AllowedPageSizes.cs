namespace RealEaseApp.Pages.Pagination;

internal class AllowedPageSizes
{
    private List<int> AllowedValues { get; }

    internal AllowedPageSizes(List<int> allowedValues)
    {
        AllowedValues = allowedValues;
    }

    internal int GetClosestPageSizeTo(int pageSize)
    {
        return AllowedValues.MinBy(allowedValue => Math.Abs(allowedValue - pageSize));
    }
}