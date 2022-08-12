using Discord.Commands;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Discord.Commands;

public class Help : ModuleBase<SocketCommandContext>
{
    private readonly IConfiguration _configuration;
    
    public Help(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [Command("help")]
    [Alias("h")]
    internal Task HelpHandler() => ReplyAsync(HelpText);
    
    private string WebsiteUrl => _configuration.GetConnectionString("Web");
    private string HelpText => "Welcome to RealEase!\n" +
                               $"Everything can be controlled via the official website {WebsiteUrl}\n\n" +
                               "Commands can be used as well:\n" +
                               "rn!subscribe \"[mediaName]\"\n" +
                               "rn!unsubscribe \"[mediaName]\"\n" +
                               "rn!listAvailable (50 max)\n" +
                               "rn!listSubscribed\n" +
                               "rn!listWebsites\n" +
                               "rn!addMedia [websiteName] [relativeUrl]\n" +
                               "\te.g.: rn!addMedia earlymanga /manga/tower-of-god\n" +
                               "rn!addScrapeTarget \"[mediaName]\" [websiteName] [relativeUrl]\n" +
                               "rn!listScrapeTargets \"[mediaName]\"";
}