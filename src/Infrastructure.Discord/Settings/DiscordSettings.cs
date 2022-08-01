namespace Infrastructure.Discord.Settings;

public record DiscordSettings
{
    public string Token { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}