using Microsoft.AspNetCore.Mvc;

namespace RealEaseApp.Pages.ViewComponents.Media;

public class AddScrapeTargetViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View("~/Pages/ViewComponents/Media/AddScrapeTargetViewComponent.cshtml");
    }
}