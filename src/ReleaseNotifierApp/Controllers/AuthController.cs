using AspNet.Security.OAuth.Discord;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ReleaseNotifierApp.Controllers;

[AllowAnonymous]
[ApiController]
[Route("signin-discord")]
public class AuthController : Controller
{
    [HttpGet]
    public IActionResult SignInDiscord()
    {
        return RedirectToPage("/Index");
    }
    
    [HttpGet("[action]")]
    public async Task Login()
    {
        await HttpContext.ChallengeAsync(DiscordAuthenticationDefaults.AuthenticationScheme, new AuthenticationProperties { RedirectUri = "http://localhost:5000" });
    }
    
    [HttpGet("[action]")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToPage("/Index");
    }
}