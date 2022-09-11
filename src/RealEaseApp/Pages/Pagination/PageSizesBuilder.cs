namespace RealEaseApp.Pages.Pagination;

internal class PageSizesBuilder
{
    private List<int> AllowedValues { get; }

    internal PageSizesBuilder(IEnumerable<int> allowedValues)
    {
        AllowedValues = allowedValues.ToList();
    }

    internal PageSizes Build(int pageSize)
    {
        var activePageSize = GetClosestAllowedPageSizeTo(pageSize);
        var pageSizeItems = AllowedValues
            .Select(allowedValue => new PageSizeItem(allowedValue, allowedValue == activePageSize))
            .ToList();

        return new PageSizes(pageSizeItems, activePageSize);

    }
    
    private int GetClosestAllowedPageSizeTo(int pageSize)
    {
        return AllowedValues.MinBy(allowedValue => Math.Abs(allowedValue - pageSize));
    }


}