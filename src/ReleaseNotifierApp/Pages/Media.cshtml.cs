using Application.UseCases.Base;
using Application.UseCases.Media.AddMedia;
using Application.UseCases.Media.QueryAvailableMedia;
using Application.UseCases.Subscriber.QueryMediaSubscriptions;
using Application.UseCases.Subscriber.SubscribeMedia;
using Application.UseCases.Subscriber.UnsubscribeMedia;
using Application.UseCases.Website.QueryAvailableWebsites;
using Domain.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ReleaseNotifierApp.Extensions;
using ReleaseNotifierApp.Pages.Pagination;

namespace ReleaseNotifierApp.Pages;

public class Media : PageModel
{
    [FromQuery] [HiddenInput] public int PageIndex { get; set; } = 1;
    public const int PageSize = 25;
    public int TotalResultCount = 0;

    public IReadOnlyCollection<MediaViewModel> MediaViewModels { get; private set; }
    public PaginationNavigation PaginationNavigation;
    // TODO: create common component for select of website Media and MediaDetails use it
    public IReadOnlyCollection<WebsiteViewModel> WebsiteViewModels { get; private set; }

    private readonly IQueryDispatcher _queryDispatcher;
    private readonly ICommandDispatcher _commandDispatcher;

    public Media(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher)
    {
        _queryDispatcher = queryDispatcher;
        _commandDispatcher = commandDispatcher;
    }

    public async Task OnGet()
    {
        await SetupPage();
    }

    public async Task<IActionResult> OnPostSubscribe(string mediaName)
    {
        var externalIdentifier = User.GetExternalIdentifier();
        var subscribeResult =
            await _commandDispatcher.Dispatch<SubscribeMediaCommand, Result>(
                new SubscribeMediaCommand(externalIdentifier, mediaName));
        // TODO: handle failure (toast or so)

        await SetupPage();
        return Page();
    }

    public async Task<IActionResult> OnPostUnsubscribe(string mediaName)
    {
        var externalIdentifier = User.GetExternalIdentifier();
        var unsubscribeResult =
            await _commandDispatcher.Dispatch<UnsubscribeMediaCommand, Result>(
                new UnsubscribeMediaCommand(externalIdentifier, mediaName));
        // TODO: handle failure (toast or so)

        await SetupPage();
        return Page();
    }

    public async Task<IActionResult> OnPostNewMedia(string websiteName, string relativePath)
    {
        // TODO: handle validation
        var addMediaCommand = new AddMediaCommand(websiteName, relativePath);

        var addMediaResult =
            await _commandDispatcher.Dispatch<AddMediaCommand, Result>(addMediaCommand);
        // TODO: handle failure (toast or so)

        await SetupPage();
        return Page();
    }

    private async Task SetupPage()
    {
        var availableMedia = await FetchMedia();
        var subscribedToMedia = await FetchSubscribedToMedia();
        var availableWebsites = await FetchAvailableWebsites();

        WebsiteViewModels = availableWebsites.Websites
            .Select(website => new WebsiteViewModel(website.Name, website.Url))
            .ToList();

        TotalResultCount = availableMedia.TotalResultCount;
        MediaViewModels = BuildMediaViewModels(
            availableMedia.Media,
            subscribedToMedia.SubscribedToMedia.Select(subscribedToMedia => subscribedToMedia.MediaId)
        );
        PaginationNavigation = PaginationNavigationBuilder.Build(
            PageIndex,
            PageSize,
            TotalResultCount
        );
    }

    private Task<AvailableMedia> FetchMedia()
    {
        return _queryDispatcher.Dispatch<AvailableMediaQuery, AvailableMedia>(
            new AvailableMediaQuery(PageIndex, PageSize));
    }

    private async Task<MediaSubscriptions> FetchSubscribedToMedia()
    {
        var externalIdentifier = User.GetExternalIdentifier();
        return await _queryDispatcher.Dispatch<MediaSubscriptionsQuery, MediaSubscriptions>(
            new MediaSubscriptionsQuery(externalIdentifier));
    }

    private static IReadOnlyCollection<MediaViewModel> BuildMediaViewModels(
        IEnumerable<MediaInformation> mediaInfos,
        IEnumerable<Guid> subscribedToMedia)
    {
        return mediaInfos.Select(mediaInfo => new MediaViewModel(
            mediaInfo.Id,
            mediaInfo.Name,
            subscribedToMedia.Contains(mediaInfo.Id))
        ).ToList();
    }

    private async Task<AvailableWebsites> FetchAvailableWebsites()
    {
        return await _queryDispatcher.Dispatch<AvailableWebsitesQuery, AvailableWebsites>(new AvailableWebsitesQuery());
    }

    public record MediaViewModel(Guid MediaId, string Name, bool UserSubscribed);
}