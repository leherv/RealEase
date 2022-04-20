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
        
        var socketUserResult = GetUser(subscriberExternalIdentifier);
        if (socketUserResult.IsFailure)
            return socketUserResult;
        
        var socketUser = socketUserResult.Value;
        var message = BuildReleasePublishedMessage(
            mediaName,
            linkToReleasedResource
        );
        
        return await NotifyUser(socketUser, message);
    }

    private async Task<Result> NotifyUser(SocketUser socketUser, string message)
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
    
    private Result<SocketUser> GetUser(string subscriberExternalIdentifier)
    {
        if (ulong.TryParse(subscriberExternalIdentifier, out var userId))
            return Errors.Notification.MalformedExternalIdentifierError(subscriberExternalIdentifier);
        var socketUser = _client.GetUser(userId);

        return socketUser == null
            ? Errors.Notification.NoSubscriberForExternalIdentifierError(subscriberExternalIdentifier)
            : socketUser;
    }

   

  
}