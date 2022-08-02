using Application.UseCases.Base;
using Application.UseCases.Subscriber.QueryMediaSubscriptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ReleaseNotifierApp.Extensions;

namespace ReleaseNotifierApp.Pages;

public class Index : PageModel
{
    private readonly IQueryDispatcher _queryDispatcher;

    [BindProperty] public List<string> CheckedMediaNames { get; set; }
    public MediaSubscriptions MediaSubscriptions { get; set; }

    public Index(IQueryDispatcher queryDispatcher)
    {
        _queryDispatcher = queryDispatcher;
    }

    public async Task<IActionResult> OnGet()
    {
        await FetchSubscriptions();

        return Page();
    }

    public void OnPostSubmit()
    {
        Console.WriteLine();
    }

    private async Task FetchSubscriptions()
    {
        var externalIdentifier = User.GetExternalIdentifier();
        
        MediaSubscriptions = await _queryDispatcher.Dispatch<MediaSubscriptionsQuery, MediaSubscriptions>(
            new MediaSubscriptionsQuery(externalIdentifier));
        
        CheckedMediaNames = MediaSubscriptions.SubscribedToMedia
            .Select(subscribedToMedia => subscribedToMedia.MediaName)
            .ToList();
    }
}