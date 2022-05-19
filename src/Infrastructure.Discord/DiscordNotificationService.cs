using Application.Ports.Notification;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Domain.ApplicationErrors;
using Domain.Results;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Discord;

public class DiscordNotificationService : INotificationService
{
    private readonly DiscordSocketClient _client;
    private readonly ILogger<DiscordNotificationService> _logger;

    public DiscordNotificationService(
        DiscordSocketClient client,
        ILogger<DiscordNotificationService> logger
    )
    {
        _client = client;
        _logger = logger;
    }

    public async Task<Result> Notify(ReleasePublishedNotification releasePublishedNotification)
    {
        var (subscriberExternalIdentifier, mediaName, linkToReleasedResource) = releasePublishedNotification;
        
        var discordUserResult = await GetDiscordUser(subscriberExternalIdentifier);
        if (discordUserResult.IsFailure)
            return discordUserResult;
        
        var discordUser = discordUserResult.Value;
        var message = BuildReleasePublishedMessage(
            mediaName,
            linkToReleasedResource
        );
        
        return await NotifyDiscordUser(discordUser, message);
    }

    private async Task<Result> NotifyDiscordUser(IUser socketUser, string message)
    {
        try
        {
            await socketUser.SendMessageAsync(message);
            return Result.Success();
        }
        catch (HttpException exception)
        {
            _logger.LogError(exception, "Notifying the user failed due to {Details}", exception.Message);
            return Result.Failure(Errors.Notification.NotifyingSubscriberFailedError(socketUser.Id.ToString()));
        }
    }
    
    private static string BuildReleasePublishedMessage(string mediaName, string linkToReleasedResource)
    {
        return $"New release for {mediaName}! Check it out at: {linkToReleasedResource}";
    }
    
    private async Task<Result<IUser>> GetDiscordUser(string subscriberExternalIdentifier)
    {
        if (!ulong.TryParse(subscriberExternalIdentifier, out var userId))
            return Errors.Notification.MalformedExternalIdentifierError(subscriberExternalIdentifier);
        var user = await _client.GetUserAsync(userId);

        return user == null
            ? Errors.Notification.NoSubscriberForExternalIdentifierError(subscriberExternalIdentifier)
            : Result<IUser>.Success(user);
    }

   

  
}