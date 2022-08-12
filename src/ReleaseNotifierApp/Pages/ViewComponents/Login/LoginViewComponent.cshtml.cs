using Microsoft.AspNetCore.Mvc;

namespace ReleaseNotifierApp.Pages.ViewComponents.Login;

public class LoginViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View("~/Pages/ViewComponents/Login/LoginViewComponent.cshtml");
    }
}