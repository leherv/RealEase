using Microsoft.AspNetCore.Mvc;

namespace RealEaseApp.Pages.ViewComponents.Media;

public class AddMediaViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View("~/Pages/ViewComponents/Media/AddMediaViewComponent.cshtml");
    }
}