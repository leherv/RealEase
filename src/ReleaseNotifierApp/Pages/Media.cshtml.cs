using Application.UseCases.Base;
using Application.UseCases.Media.QueryAvailableMedia;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ReleaseNotifierApp.Pages.Pagination;

namespace ReleaseNotifierApp.Pages;

public class Media : PageModel
{
    [FromQuery]
    public int PageIndex { get; set; } = 1;
    public int PageSize = 1;
    
    public int TotalResultCount = 0;
    
    public IReadOnlyCollection<MediaInformation> MediaInfos;
    public PaginationNavigation PaginationNavigation;
    
    private readonly IQueryDispatcher _queryDispatcher;

    public Media(IQueryDispatcher queryDispatcher)
    {
        _queryDispatcher = queryDispatcher;
    }

    public async Task OnGet()
    {
        var availableMedia = await FetchMedia();
        MediaInfos = availableMedia.Media
            .OrderBy(media => media.Name)
            .ToList();
        TotalResultCount = availableMedia.TotalResultCount;

        PaginationNavigation = PaginationNavigationBuilder.Build(PageIndex, PageSize, TotalResultCount);
    }

    private Task<AvailableMedia> FetchMedia()
    {
        return _queryDispatcher.Dispatch<AvailableMediaQuery, AvailableMedia>(new AvailableMediaQuery(PageIndex, PageSize));
    }
}