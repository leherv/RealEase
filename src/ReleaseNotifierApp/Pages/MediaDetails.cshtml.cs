using Application.UseCases.Base;
using Application.UseCases.Media.AddScrapeTarget;
using Application.UseCases.Media.QueryMedia;
using Application.UseCases.Website.QueryAvailableWebsites;
using Domain.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ReleaseNotifierApp.Pages;

public class MediaDetailsModel : PageModel
{
    [BindProperty(SupportsGet = true)] public Guid Id { get; set; }

    public IReadOnlyCollection<WebsiteViewModel> WebsiteViewModels { get; private set; }

    private MediaDetails MediaDetails;

    private readonly IQueryDispatcher _queryDispatcher;
    private readonly ICommandDispatcher _commandDispatcher;

    public MediaDetailsModel(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher)
    {
        _queryDispatcher = queryDispatcher;
        _commandDispatcher = commandDispatcher;
    }

    public async Task OnGet()
    {
        await SetupPage();
    }

    public async Task<IActionResult> OnPost(string mediaName, string websiteName, string relativePath)
    {
        // TODO: handle validation
        var addScrapeTargetResult =
            await _commandDispatcher.Dispatch<AddScrapeTargetCommand, Result>(
                new AddScrapeTargetCommand(mediaName, websiteName, relativePath));
        // TODO: handle failure (toast or so)

        await SetupPage();
        return Page();
    }

    private async Task SetupPage()
    {
        var mediaDetailsResult = await FetchMediaDetails();
        // TODO: handle failure (toast or so)
        MediaDetails = mediaDetailsResult.Value;

        var availableWebsites = await FetchAvailableWebsites();

        WebsiteViewModels = availableWebsites.Websites
            .Where(website => !MediaDetails.ScrapeTargetDetails
                .Select(scrapeTargetDetail => scrapeTargetDetail.WebsiteName)
                .Contains(website.Name, StringComparer.InvariantCultureIgnoreCase))
            .Select(website => new WebsiteViewModel(website.Name, website.Url))
            .ToList();
    }

    private async Task<Result<MediaDetails>> FetchMediaDetails()
    {
        return await _queryDispatcher.Dispatch<MediaQuery, Result<MediaDetails>>(new MediaQuery(Id));
    }

    private async Task<AvailableWebsites> FetchAvailableWebsites()
    {
        return await _queryDispatcher.Dispatch<AvailableWebsitesQuery, AvailableWebsites>(new AvailableWebsitesQuery());
    }

    public string MediaName => MediaDetails.Name;

    public bool HasRelease => MediaDetails.ReleaseDetails != null;

    public bool HasScrapeTargets => MediaDetails.ScrapeTargetDetails.Any();

    public IReadOnlyCollection<ScrapeTargetDetails> ScrapeTargetDetails => MediaDetails.ScrapeTargetDetails;

    public string LatestReleaseDisplayString()
    {
        if (MediaDetails.ReleaseDetails == null)
            return "No Release scraped yet";

        var result = $"Chapter {MediaDetails.ReleaseDetails.LatestReleaseMajor}";
        if (MediaDetails.ReleaseDetails.LatestReleaseMinor > 0)
            result += $".{MediaDetails.ReleaseDetails.LatestReleaseMinor}";

        return result;
    }

    public string NewestChapterLink => HasRelease
        ? MediaDetails.ReleaseDetails.LatestReleaseUrl
        : "";
}