using Application.UseCases.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ReleaseNotifierApp.Pages;

public class Media : PageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }
    
    private readonly IQueryDispatcher _queryDispatcher;
    public Media(IQueryDispatcher queryDispatcher)
    {
        _queryDispatcher = queryDispatcher;
    }
    
    public async Task<IActionResult> OnGet()
    {
        return Page();
    }
}