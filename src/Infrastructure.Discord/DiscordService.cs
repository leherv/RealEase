using Discord;
using Discord.WebSocket;
using Infrastructure.Discord.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Discord;

public class DiscordService : BackgroundService
{
    private readonly ILogger<DiscordService> _logger;
    private readonly DiscordSocketClient _client;
    private readonly CommandHandlingService _commandHandlingService;
    private readonly DiscordSettings _discordSettings;

    public DiscordService(
        DiscordSocketClient discordSocketClient,
        CommandHandlingService commandHandlingService,
        ILogger<DiscordService> logger,
        IOptions<DiscordSettings> discordSettings
    )
    {
        _commandHandlingService = commandHandlingService;
        _logger = logger;
        _discordSettings = discordSettings.Value;
        _client = discordSocketClient;
        _client.Log += Log;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Connecting to Discord");
            await _client.LoginAsync(TokenType.Bot, _discordSettings.Token);
            await _client.StartAsync();
            await _commandHandlingService.InitializeAsync();
            _logger.LogInformation("Connection successful.");
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong: {exception}\n {innerException}", e.Message, e.InnerException?.Message);
        }
    }
    
    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
}