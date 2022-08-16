using System.ComponentModel.DataAnnotations;
using Application.UseCases.Base;
using Application.UseCases.Media.AddScrapeTarget;
using Application.UseCases.Media.DeleteScrapeTarget;
using Application.UseCases.Media.QueryMedia;
using Application.UseCases.Website.QueryAvailableWebsites;
using AspNetCoreHero.ToastNotification.Abstractions;
using Domain.ApplicationErrors;
using Domain.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RealEaseApp.Pages;

public class MediaDetailsModel : PageModel
{
    [BindProperty(SupportsGet = true)] 
    public Guid Id { get; set; }

    public MediaDetailsViewModel? MediaDetailsViewModel { get; private set; }
    public IReadOnlyCollection<WebsiteViewModel> WebsiteViewModels { get; private set; }

    private readonly IQueryDispatcher _queryDispatcher;
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IToastifyService _toastifyService;
    private readonly ILogger<MediaDetailsModel> _logger;

    public MediaDetailsModel(
        IQueryDispatcher queryDispatcher,
        ICommandDispatcher commandDispatcher,
        IToastifyService toastifyService,
        ILogger<MediaDetailsModel> logger
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

    public async Task<IActionResult> OnPostNewScrapeTarget(NewScrapeTarget newScrapeTarget)
    {
        if (ModelState.IsValid)
        {
            var addScrapeTargetResult =
                await _commandDispatcher.Dispatch<AddScrapeTargetCommand, Result>(
                    new AddScrapeTargetCommand(
                        newScrapeTarget.MediaName,
                        newScrapeTarget.WebsiteName,
                        newScrapeTarget.RelativePath
                    ));

            if (addScrapeTargetResult.IsFailure)
            {
                _logger.LogError(addScrapeTargetResult.Error.ToString());
                _toastifyService.Error(BuildNewScrapeTargetErrorMessage(addScrapeTargetResult));
            }
        }

        await SetupPage();
        return Page();
    }

    public async Task<IActionResult> OnPostDelete(Guid mediaId, Guid scrapeTargetId)
    {
        var deleteScrapeTargetCommand = new DeleteScrapeTargetCommand(mediaId, scrapeTargetId);
        
        var deleteScrapeTargetResult = await _commandDispatcher.Dispatch<DeleteScrapeTargetCommand, Result>(deleteScrapeTargetCommand);
        if (deleteScrapeTargetResult.IsFailure)
        {
            _logger.LogError(deleteScrapeTargetResult.Error.ToString());
            _toastifyService.Error(BuildDeleteScrapeTargetErrorMessage(deleteScrapeTargetResult));
        }
        
        await SetupPage();
        return Page();
    }

    private static string BuildDeleteScrapeTargetErrorMessage(Result result) =>
        result.Error.Code switch
        {
            Errors.General.NotFoundErrorCode => "Entity was not found",
            _ => "Something went wrong"
        };

    private static string BuildNewScrapeTargetErrorMessage(Result result) =>
        result.Error.Code switch
        {
            Errors.General.NotFoundErrorCode => "Entity was not found",
            Errors.Validation.InvariantViolationErrorCode => "Creating entity failed",
            Errors.Media.ScrapeTargetExistsErrorCode => "ScrapeTarget already exists",
            Errors.Scraper.ScrapeFailedErrorCode => "Scraping for media failed. It is likely this is due to a problem with the target site. Please try again later.",
            Errors.Scraper.ScrapeMediaNameFailedErrorCode => "Scraping for media name failed. It is likely this is due to a problem with the target site. Please try again later.",
            Errors.Media.ScrapeTargetReferencesOtherMediaErrorCode => "ScrapeTarget references different media",
            _ => "Something went wrong"
        };

    private async Task SetupPage()
    {
        var mediaDetailsResult = await FetchMediaDetails();
        if (mediaDetailsResult.IsFailure)
        {
            _logger.LogError(mediaDetailsResult.Error.ToString());
            _toastifyService.Error(BuildErrorMessage(mediaDetailsResult));
            MediaDetailsViewModel = new MediaDetailsViewModel(
                Guid.NewGuid(),
                "",
                "",
                "",
                new List<ScrapeTargetDetailsViewModel>());
            WebsiteViewModels = new List<WebsiteViewModel>();
        }
        else
        {
            var mediaDetails = mediaDetailsResult.Value;
            MediaDetailsViewModel = BuildMediaDetailsViewModel(mediaDetails);
            WebsiteViewModels = await BuildWebsiteViewModel(mediaDetails);
        }
    }

    private static string BuildErrorMessage(Result<MediaDetails> mediaDetailsResult)
    {
        return mediaDetailsResult.Error.Code == Errors.General.NotFoundErrorCode
            ? "Media not found"
            : "Something went wrong";
    }

    private async Task<Result<MediaDetails>> FetchMediaDetails()
    {
        return await _queryDispatcher.Dispatch<MediaQuery, Result<MediaDetails>>(new MediaQuery(Id));
    }

    private static MediaDetailsViewModel BuildMediaDetailsViewModel(MediaDetails mediaDetails)
    {
        var scrapeTargetViewModels = mediaDetails.ScrapeTargetDetails
            .Select(scrapeTargetDetail => new ScrapeTargetDetailsViewModel(
                scrapeTargetDetail.ScrapeTargetId,
                scrapeTargetDetail.WebsiteName,
                scrapeTargetDetail.WebsiteUrl,
                scrapeTargetDetail.ScrapeTargetUrl))
            .ToList();

        return new MediaDetailsViewModel(
            mediaDetails.Id,
            mediaDetails.Name,
            LatestReleaseDisplayString(mediaDetails.ReleaseDetails),
            mediaDetails.ReleaseDetails != null
                ? mediaDetails.ReleaseDetails.LatestReleaseUrl
                : "",
            scrapeTargetViewModels
        );
    }

    private async Task<IReadOnlyCollection<WebsiteViewModel>> BuildWebsiteViewModel(MediaDetails mediaDetails)
    {
        var availableWebsites = await FetchAvailableWebsites();
        return availableWebsites.Websites
            .Where(website => !mediaDetails.ScrapeTargetDetails
                .Select(scrapeTargetDetail => scrapeTargetDetail.WebsiteName)
                .Contains(website.Name, StringComparer.InvariantCultureIgnoreCase))
            .Select(website => new WebsiteViewModel(website.Id, website.Name, website.Url))
            .ToList();
    }

    private async Task<AvailableWebsites> FetchAvailableWebsites()
    {
        return await _queryDispatcher.Dispatch<AvailableWebsitesQuery, AvailableWebsites>(new AvailableWebsitesQuery());
    }

    private static string LatestReleaseDisplayString(ReleaseDetails? releaseDetails)
    {
        if (releaseDetails == null)
            return "No Release scraped yet";

        var result = $"Chapter {releaseDetails.LatestReleaseMajor}";
        if (releaseDetails.LatestReleaseMinor > 0)
            result += $".{releaseDetails.LatestReleaseMinor}";

        return result;
    }
}

public record MediaDetailsViewModel(
    Guid MediaId,
    string MediaName,
    string LatestRelease,
    string NewestChapterLink,
    IReadOnlyCollection<ScrapeTargetDetailsViewModel> ScrapeTargetDetailsViewModels
)
{
    public bool HasRelease => !string.IsNullOrEmpty(LatestRelease);
    public bool HasScrapeTargets => ScrapeTargetDetailsViewModels.Any();
}

public record ScrapeTargetDetailsViewModel(
    Guid ScrapeTargetId,
    string WebsiteName,
    string WebsiteUrl,
    string ScrapeTargetUrl
);

public record NewScrapeTarget
{
    [Required] public string MediaName { get; set; }

    [Required] public string WebsiteName { get; set; }

    [Required] public string RelativePath { get; set; }
}