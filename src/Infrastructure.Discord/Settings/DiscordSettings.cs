namespace Infrastructure.Discord.Settings;

public record DiscordSettings
{
    public string Token { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string AddBotUri { get; set; }
    public string RedirectUri { get; set; }
}