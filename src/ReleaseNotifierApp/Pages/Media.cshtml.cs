using System.ComponentModel.DataAnnotations;
using Application.UseCases.Base;
using Application.UseCases.Media.AddMedia;
using Application.UseCases.Media.QueryAvailableMedia;
using Application.UseCases.Subscriber.QueryMediaSubscriptions;
using Application.UseCases.Subscriber.SubscribeMedia;
using Application.UseCases.Subscriber.UnsubscribeMedia;
using Application.UseCases.Website.QueryAvailableWebsites;
using AspNetCoreHero.ToastNotification.Abstractions;
using Domain.ApplicationErrors;
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
    private readonly IToastifyService _toastifyService;

    public Media(
        IQueryDispatcher queryDispatcher,
        ICommandDispatcher commandDispatcher,
        IToastifyService toastifyService
    )
    {
        _queryDispatcher = queryDispatcher;
        _commandDispatcher = commandDispatcher;
        _toastifyService = toastifyService;
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

        if (subscribeResult.IsFailure)
        {
            _toastifyService.Error(BuildSubscribeErrorMessage(subscribeResult));
        }

        await SetupPage();
        return Page();
    }
    
    public async Task<IActionResult> OnPostUnsubscribe(string mediaName)
    {
        var externalIdentifier = User.GetExternalIdentifier();
        var unsubscribeResult =
            await _commandDispatcher.Dispatch<UnsubscribeMediaCommand, Result>(
                new UnsubscribeMediaCommand(externalIdentifier, mediaName));
        
        if (unsubscribeResult.IsFailure)
        {
            _toastifyService.Error(BuildUnsubscribeErrorMessage(unsubscribeResult));
        }

        await SetupPage();
        return Page();
    }
    
    public async Task<IActionResult> OnPostNewMedia(NewMedia newMedia)
    {
        if (ModelState.IsValid)
        {
            var addMediaCommand = new AddMediaCommand(newMedia.WebsiteName, newMedia.RelativePath);

            var addMediaResult =
                await _commandDispatcher.Dispatch<AddMediaCommand, Result>(addMediaCommand);
            
            if (addMediaResult.IsFailure)
            {
                _toastifyService.Error(addMediaResult.Error.ToString());
            }
        }
        
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
    
    private static string BuildSubscribeErrorMessage(Result result) =>
        result.Error.Code switch
        {
            Errors.General.NotFoundErrorCode => "Entity was not found",
            Errors.Validation.InvariantViolationErrorCode => "Creating entity failed",
            _ => "Something went wrong"
        };
    
    private static string BuildUnsubscribeErrorMessage(Result result) =>
        result.Error.Code switch
        {
            Errors.General.NotFoundErrorCode => "Entity was not found",
            Errors.Subscriber.UnsubscribeFailedErrorCode => "Unsubscribing failed",
            _ => "Something went wrong"
        };
    
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
    
    private async Task<AvailableWebsites> FetchAvailableWebsites()
    {
        return await _queryDispatcher.Dispatch<AvailableWebsitesQuery, AvailableWebsites>(new AvailableWebsitesQuery());
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

    public record MediaViewModel(Guid MediaId, string Name, bool UserSubscribed);

    public record NewMedia
    {
        [Required]
        public string WebsiteName { get; set; }

        [Required] 
        public string RelativePath { get; set; }
    }
}