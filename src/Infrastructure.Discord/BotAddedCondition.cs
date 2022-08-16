using AspNet.Security.OAuth.Discord;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Discord;

public class BotAddedCondition
{
    private readonly DiscordSocketClient _socketClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BotAddedCondition(DiscordSocketClient socketClient, IHttpContextAccessor httpContextAccessor)
    {
        _socketClient = socketClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> Evaluate(ulong currentUserId)
    {
        await EnsureLoggedIn();
        var botGuilds = await _socketClient.Rest.GetGuildsAsync();

        return await UserIsInAnyGuild(botGuilds, currentUserId);
    }

    private async Task EnsureLoggedIn()
    {
        if (_socketClient.LoginState != LoginState.LoggedIn)
        {
            var token = await GetToken();
            await _socketClient.LoginAsync(TokenType.Bearer, token);
        }
    }

    private async Task<string?> GetToken()
    {
        return await _httpContextAccessor.HttpContext!
            .GetTokenAsync(DiscordAuthenticationDefaults.AuthenticationScheme, "access_token");
    }

    private static async Task<bool> UserIsInAnyGuild(IReadOnlyCollection<RestGuild> botGuilds, ulong currentUserId)
    {
        foreach (var botGuild in botGuilds)
        {
            if (await UserIsInGuild(currentUserId, botGuild)) return true;
        }

        return false;
    }

    private static async Task<bool> UserIsInGuild(ulong currentUserId, RestGuild botGuild)
    {
        var currentUser = await botGuild.GetUserAsync(currentUserId);
        return currentUser != null;
    }
}