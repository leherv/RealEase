using System.ComponentModel.DataAnnotations;
using Application.UseCases.Base;
using Application.UseCases.Media.AddMedia;
using Application.UseCases.Media.DeleteMedia;
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
using RealEaseApp.Pages.Pagination;
using RealEaseApp.Extensions;

namespace RealEaseApp.Pages;

public class Media : PageModel
{
    [FromQuery] [HiddenInput] public int PageIndex { get; set; } = 1;
    [FromQuery] [HiddenInput] public string QueryString { get; set; } = "";
    private const int PageSize = 10;
    private int _totalResultCount = 0;

    public IReadOnlyCollection<MediaViewModel> MediaViewModels { get; private set; }
    public PaginationNavigation PaginationNavigation;
    public IReadOnlyCollection<WebsiteViewModel> WebsiteViewModels { get; private set; }

    private readonly IQueryDispatcher _queryDispatcher;
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IToastifyService _toastifyService;
    private readonly ILogger<Media> _logger;

    public Media(
        IQueryDispatcher queryDispatcher,
        ICommandDispatcher commandDispatcher,
        IToastifyService toastifyService,
        ILogger<Media> logger
    )
    {
        _queryDispatcher = queryDispatcher;
        _commandDispatcher = commandDispatcher;
        _toastifyService = toastifyService;
        _logger = logger;
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
            _logger.LogError(subscribeResult.Error.ToString());
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
            _logger.LogError(unsubscribeResult.Error.ToString());
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
                _logger.LogError(addMediaResult.Error.ToString());
                _toastifyService.Error(BuildAddMediaErrorMessage(addMediaResult));
            }
        }
        
        await SetupPage();
        return Page();
    }

    public async Task<IActionResult> OnPostDelete(Guid mediaId)
    {
        var deleteMediaCommand = new DeleteMediaCommand(mediaId, User.GetExternalIdentifier());
        var deleteMediaResult = await _commandDispatcher.Dispatch<DeleteMediaCommand, Result>(deleteMediaCommand);
        
        if (deleteMediaResult.IsFailure)
        {
            _logger.LogError(deleteMediaResult.Error.ToString());
            _toastifyService.Error(BuildDeleteMediaErrorMessage(deleteMediaResult));
        }
        
        await SetupPage();
        return Page();
    }
    
    public async Task<IActionResult> OnPostSearch(string mediaNameSearchString)
    {
        QueryString = mediaNameSearchString;
        await SetupPage();
        return Page();
    }
    
    private static string BuildAddMediaErrorMessage(Result result) =>
        result.Error.Code switch
        {
            Errors.General.NotFoundErrorCode => "Website was not found",
            Errors.Media.MediaWithScrapeTargetExistsErrorCode => "Media for this URL exists",
            Errors.Media.MediaWithNameExistsErrorCode => "Media with this name exists",
            Errors.Scraper.ScrapeFailedErrorCode => "Scraping for media failed. It is likely this is due to a problem with the target site. Please try again later.",
            Errors.Scraper.ScrapeMediaNameFailedErrorCode => "Scraping for media name failed. It is likely this is due to a problem with the target site. Please try again later.",
            Errors.Validation.InvariantViolationErrorCode => "Creating entity failed",
            _ => "Something went wrong"
        };

    private static string BuildDeleteMediaErrorMessage(Result result) =>
        result.Error.Code switch
        {
            Errors.General.NotFoundErrorCode => "Media not found",
            _ => "Something went wrong"
        };
    
    private async Task SetupPage()
    {
        var availableMedia = await FetchMedia(QueryString);
        var subscribedToMedia = await FetchSubscribedToMedia();
        var availableWebsites = await FetchAvailableWebsites();

        WebsiteViewModels = availableWebsites.Websites
            .Select(website => new WebsiteViewModel(website.Id, website.Name, website.Url))
            .ToList();

        _totalResultCount = availableMedia.TotalResultCount;
        MediaViewModels = BuildMediaViewModels(
            availableMedia.Media,
            subscribedToMedia.SubscribedToMedia.Select(subscribedToMedia => subscribedToMedia.MediaId)
        );
        PaginationNavigation = PaginationNavigationBuilder.Build(
            PageIndex,
            PageSize,
            _totalResultCount
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
    
    private Task<AvailableMedia> FetchMedia(string mediaNameSearchString)
    {
        return _queryDispatcher.Dispatch<AvailableMediaQuery, AvailableMedia>(
            new AvailableMediaQuery(PageIndex, PageSize, mediaNameSearchString));
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