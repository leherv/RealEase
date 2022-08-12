using Microsoft.AspNetCore.Mvc;

namespace RealEaseApp.Pages.ViewComponents.Login;

public class LoginViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View("~/Pages/ViewComponents/Login/LoginViewComponent.cshtml");
    }
}