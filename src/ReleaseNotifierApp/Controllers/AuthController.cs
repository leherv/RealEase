using AspNet.Security.OAuth.Discord;
using Infrastructure.Discord.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ReleaseNotifierApp.Controllers;

[AllowAnonymous]
[ApiController]
[Route("signin-discord")]
public class AuthController : Controller
{
    private readonly DiscordSettings _discordSettings;

    public AuthController(IOptions<DiscordSettings> discordSettings)
    {
        _discordSettings = discordSettings.Value;
    }

    [HttpGet]
    public IActionResult SignInDiscord()
    {
        return RedirectToPage("/Index");
    }
    
    [HttpGet("[action]")]
    public async Task Login()
    {
        await HttpContext.ChallengeAsync(DiscordAuthenticationDefaults.AuthenticationScheme, new AuthenticationProperties { RedirectUri = _discordSettings.RedirectUri });
    }
    
    [HttpGet("[action]")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToPage("/Index");
    }
}