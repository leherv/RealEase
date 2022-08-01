using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ReleaseNotifierApp.Extensions;

namespace ReleaseNotifierApp.Pages;

public class Login : PageModel
{
    public IActionResult OnGet()
    {
        if (User.IsAuthenticated())
        {
            return RedirectToPage("/Index");
        }

        return Page();
    }
}