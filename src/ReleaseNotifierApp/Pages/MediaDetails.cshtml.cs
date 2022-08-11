using System.ComponentModel.DataAnnotations;
using Application.UseCases.Base;
using Application.UseCases.Media.AddScrapeTarget;
using Application.UseCases.Media.QueryMedia;
using Application.UseCases.Website.QueryAvailableWebsites;
using AspNetCoreHero.ToastNotification.Abstractions;
using Domain.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ReleaseNotifierApp.Pages;

public class MediaDetailsModel : PageModel
{
    [BindProperty(SupportsGet = true)] public Guid Id { get; set; }

    public MediaDetailsViewModel? MediaDetailsViewModel { get; private set; }
    public IReadOnlyCollection<WebsiteViewModel> WebsiteViewModels { get; private set; }

    private readonly IQueryDispatcher _queryDispatcher;
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly INotyfService _notyfService;

    public MediaDetailsModel(
        IQueryDispatcher queryDispatcher,
        ICommandDispatcher commandDispatcher,
        INotyfService notyfService
    )
    {
        _queryDispatcher = queryDispatcher;
        _commandDispatcher = commandDispatcher;
        _notyfService = notyfService;
    }

    public async Task OnGet()
    {
        await SetupPage();
    }

    public async Task<IActionResult> OnPost(NewScrapeTarget newScrapeTarget)
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
                _notyfService.Error(addScrapeTargetResult.Error.ToString());
            }
        }

        await SetupPage();
        return Page();
    }

    private async Task SetupPage()
    {
        var mediaDetailsResult = await FetchMediaDetails();
        if (mediaDetailsResult.IsFailure)
        {
            _notyfService.Success(mediaDetailsResult.Error.ToString());
            MediaDetailsViewModel = new MediaDetailsViewModel(
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

    private async Task<Result<MediaDetails>> FetchMediaDetails()
    {
        return await _queryDispatcher.Dispatch<MediaQuery, Result<MediaDetails>>(new MediaQuery(Id));
    }

    private static MediaDetailsViewModel BuildMediaDetailsViewModel(MediaDetails mediaDetails)
    {
        var scrapeTargetViewModels = mediaDetails.ScrapeTargetDetails
            .Select(scrapeTargetDetail => new ScrapeTargetDetailsViewModel(
                scrapeTargetDetail.WebsiteName,
                scrapeTargetDetail.WebsiteUrl,
                scrapeTargetDetail.ScrapeTargetUrl))
            .ToList();

        return new MediaDetailsViewModel(
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
            .Select(website => new WebsiteViewModel(website.Name, website.Url))
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
    string MediaName,
    string LatestRelease,
    string NewestChapterLink,
    IReadOnlyCollection<ScrapeTargetDetailsViewModel> ScrapeTargetDetailsViewModels)
{
    public bool HasRelease => !string.IsNullOrEmpty(LatestRelease);
    public bool HasScrapeTargets => ScrapeTargetDetailsViewModels.Any();
}

public record ScrapeTargetDetailsViewModel(string WebsiteName, string WebsiteUrl, string ScrapeTargetUrl);

public record NewScrapeTarget
{
    [Required] public string MediaName { get; set; }

    [Required] public string WebsiteName { get; set; }

    [Required] public string RelativePath { get; set; }
}