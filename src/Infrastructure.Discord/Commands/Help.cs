using Discord.Commands;

namespace Infrastructure.Discord.Commands;

public class Help : ModuleBase<SocketCommandContext>
{
    private const string HelpText =
        "Welcome to Vik Release Notifier (VRN)!\n" +
        "The following commands are available:\n" +
        "rn!subscribe \"[mediaName]\"\n" +
        "rn!unsubscribe \"[mediaName]\"\n" +
        "rn!listAvailable\n" +
        "rn!listSubscribed\n" +
        "rn!listWebsites\n" +
        "rn!addMedia [websiteName] [relativeUrl]\n"
        +"\te.g.: rn!addMedia earlymanga /manga/tower-of-god\n" +
        "rn!addScrapeTarget \"[mediaName]\" [websiteName] [relativeUrl]\n" +
        "rn!listScrapeTargets \"[mediaName]\"";
    
    [Command("help")]
    [Alias("h")]
    internal Task HelpHandler() => ReplyAsync(HelpText);
}