using Application.UseCases.Base;
using Application.UseCases.Media.QueryMedia;
using Domain.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ReleaseNotifierApp.Pages;

public class Media : PageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }
    public string ErrorMessage { get; set; }
    
    private MediaDetails MediaDetails;
    
    private readonly IQueryDispatcher _queryDispatcher;
    
    public Media(IQueryDispatcher queryDispatcher)
    {
        _queryDispatcher = queryDispatcher;
    }
    
    public async Task<IActionResult> OnGet()
    {
        var mediaDetailsResult = await _queryDispatcher.Dispatch<MediaQuery, Result<MediaDetails>>(new MediaQuery(Id));
        if (mediaDetailsResult.IsFailure)
            ErrorMessage = mediaDetailsResult.ToString();

        MediaDetails = mediaDetailsResult.Value;
        
        return Page();
    }

    public string MediaName => MediaDetails.Name;

    public bool HasRelease => MediaDetails.ReleaseDetails != null;

    public bool HasScrapeTargets => MediaDetails.ScrapeTargetDetails.Any();

    public IReadOnlyCollection<ScrapeTargetDetails> ScrapeTargetDetails => MediaDetails.ScrapeTargetDetails;

    public string LatestReleaseDisplayString()
    {
        if (MediaDetails.ReleaseDetails == null)
            return "No Release scraped yet";

        var result = $"{MediaDetails.ReleaseDetails.LatestReleaseMajor}";
        if (MediaDetails.ReleaseDetails.LatestReleaseMinor > 0)
            result += $".{MediaDetails.ReleaseDetails.LatestReleaseMinor}";
        
        return result;
    }

    public string NewestChapterLink => HasRelease
        ? MediaDetails.ReleaseDetails.LatestReleaseUrl
        : "";
}