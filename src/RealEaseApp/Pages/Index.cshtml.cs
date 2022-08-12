using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RealEaseApp.Pages;

public class Index : PageModel
{
  public IActionResult OnGet()
  {
    return RedirectToPage("/Media");
  }
}