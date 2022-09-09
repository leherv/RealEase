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
                               "re!subscribe \"[mediaName]\"\n" +
                               "re!unsubscribe \"[mediaName]\"\n" +
                               "re!listAvailable (50 max)\n" +
                               "re!listSubscribed\n" +
                               "re!listWebsites\n" +
                               "re!addMedia [websiteName] [relativeUrl]\n" +
                               "\te.g.: re!addMedia earlymanga /manga/tower-of-god\n" +
                               "re!addScrapeTarget \"[mediaName]\" [websiteName] [relativeUrl]\n" +
                               "re!listScrapeTargets \"[mediaName]\"";
}