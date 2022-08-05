using Infrastructure.Discord.Settings;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace ReleaseNotifierApp.Pages;

public class AddBot : PageModel
{
    private readonly DiscordSettings _discordSettings;
    public string AddBotUri => _discordSettings.AddBotUri;

    public AddBot(IOptions<DiscordSettings> discordSettings)
    {
        _discordSettings = discordSettings.Value;
    }

    public void OnGet()
    {
        
    }
}