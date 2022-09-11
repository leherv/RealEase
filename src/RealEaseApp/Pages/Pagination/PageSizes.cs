namespace RealEaseApp.Pages.Pagination;

public record PageSizes(IReadOnlyCollection<PageSizeItem> PageSizeItems, int ActivePageSize);