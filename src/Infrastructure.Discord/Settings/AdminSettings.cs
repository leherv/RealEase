namespace Infrastructure.Discord.Settings;

public record AdminSettings
{
    public AdminSettings() {}
    
    public AdminSettings(string discordAdminIdentifiers)
    {
        DiscordAdminIdentifiers = discordAdminIdentifiers;
    }

    private string DiscordAdminIdentifiers { get; init; }

    public IEnumerable<string> DiscordAdminIds => DiscordAdminIdentifiers.Split(";");
}