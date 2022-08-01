using Application.UseCases.Base;
using Application.UseCases.Subscriber.QueryMediaSubscriptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ReleaseNotifierApp.Extensions;

namespace ReleaseNotifierApp.Pages;

public class Index : PageModel
{
    private readonly IQueryDispatcher _queryDispatcher;
    
    [BindProperty]
    public List<string> CheckedMediaNames { get; set; }

    public Index(IQueryDispatcher queryDispatcher)
    {
        _queryDispatcher = queryDispatcher;
    }

    public async Task<IActionResult> OnGet()
    {
        if (!User.IsAuthenticated())
        {
            return RedirectToPage("/Login");
        }

        CheckedMediaNames = await BuildSubscriptions();
        
        return Page();
    }

    public void OnPostSubmit()
    {
        Console.WriteLine();
    }

    private async Task<List<string>> BuildSubscriptions()
    {
        var externalIdentifier = User.GetExternalIdentifier();
        var mediaSubscriptions =
            await _queryDispatcher.Dispatch<MediaSubscriptionsQuery, MediaSubscriptions>(
                new MediaSubscriptionsQuery(externalIdentifier));

        return mediaSubscriptions.SubscribedToMediaNames.ToList();
    }
}

