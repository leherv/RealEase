using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ReleaseNotifierApp.Pages;

public class Index : PageModel
{
  public IActionResult OnGet()
  {
    return RedirectToPage("/Media");
  }
}