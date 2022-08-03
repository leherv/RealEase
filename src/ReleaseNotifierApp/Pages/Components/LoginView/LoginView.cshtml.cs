using Microsoft.AspNetCore.Mvc;

namespace ReleaseNotifierApp.Pages.Components.LoginView;

public class LoginView : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View("~/Pages/Components/LoginView/LoginView.cshtml");
    }
}