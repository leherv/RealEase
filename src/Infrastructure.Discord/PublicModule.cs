using Application.UseCases.Base;
using Application.UseCases.Media;
using Application.UseCases.Subscriber.QueryMediaSubscriptions;
using Application.UseCases.Subscriber.SubscribeMedia;
using Application.UseCases.Subscriber.UnsubscribeMedia;
using Application.UseCases.Website;
using Discord.Commands;
using Domain.Results;
using Infrastructure.Discord.Extensions;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Discord;

public class PublicModule : ModuleBase<SocketCommandContext>
{
    private const string HelpText =
        "Welcome to Vik Release Notifier (VRN)!\n" +
        "The following commands are available:\n" +
        "!subscribe [mediaName]\n" +
        "!unsubscribe [mediaName]\n" +
        "!listAvailable\n" +
        "!listSubscribed\n" +
        "!listWebsites";

    private readonly IQueryDispatcher _queryDispatcher;
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly ILogger<PublicModule> _logger;

    public PublicModule(
        IQueryDispatcher queryDispatcher,
        ICommandDispatcher commandDispatcher,
        ILogger<PublicModule> logger
    )
    {
        _queryDispatcher = queryDispatcher;
        _commandDispatcher = commandDispatcher;
        _logger = logger;
    }

    [Command("help")]
    [Alias("h")]
    public Task Help() => ReplyAsync(HelpText);

    [Command("listAvailable")]
    [Alias("l")]
    public async Task ListAvailable()
    {
        var availableMedia =
            await _queryDispatcher.Dispatch<AvailableMediaQuery, AvailableMedia>(new AvailableMediaQuery());

        var message = availableMedia.MediaNames.Any()
            ? "Available Media: \n" +
              $"{string.Join("\n", availableMedia.MediaNames)}"
            : "No media available.";

        await Context.Message.Channel.SendMessageAsync(message);
    }

    [Command("listSubscribed")]
    [Alias("ls")]
    public async Task ListSubscribed()
    {
        var mediaSubscriptions =
            await _queryDispatcher.Dispatch<MediaSubscriptionsQuery, MediaSubscriptions>(
                new MediaSubscriptionsQuery(Context.User.Id.ToString()));

        var message = mediaSubscriptions.SubscribedToMediaNames.Any()
            ? "No subscriptions yet"
            : $"Subscribed To:\n{string.Join("\n", mediaSubscriptions.SubscribedToMediaNames)}";

        await Context.Message.Channel.SendMessageAsync(message);
    }

    [Command("subscribe")]
    [Alias("s")]
    public async Task Subscribe(string mediaName)
    {
        var subscribeResult =
            await _commandDispatcher.Dispatch<SubscribeMediaCommand, Result>(
                new SubscribeMediaCommand(Context.GetUserId().ToString(), mediaName));

        var message = "Done.";
        if (subscribeResult.IsFailure)
        {
            _logger.LogInformation(subscribeResult.Error.ToString());
            message = "Subscribe failed.";
        }
        
        await Context.Message.Channel.SendMessageAsync(message);
    }

    [Command("unsubscribe")]
    [Alias("us")]
    public async Task Unsubscribe(string mediaName)
    {
        var unsubscribeResult =
            await _commandDispatcher.Dispatch<UnsubscribeMediaCommand, Result>(
                new UnsubscribeMediaCommand(Context.GetUserId().ToString(), mediaName));

        var message = "Done.";
        if (unsubscribeResult.IsFailure)
        {
            _logger.LogInformation(unsubscribeResult.Error.ToString());
            message = "Unsubscribe failed.";
        }
        
        await Context.Message.Channel.SendMessageAsync(message);
    }
    
    [Command("listWebsites")]
    [Alias("lW")]
    public async Task ListWebsites()
    {
        var availableWebsites =
            await _queryDispatcher.Dispatch<AvailableWebsitesQuery, AvailableWebsites>(new AvailableWebsitesQuery());

        var message = availableWebsites.Websites.Any()
            ? "Available Websites:" +
              availableWebsites.Websites.Aggregate("", (current, availableWebsite) => current + $"\n{availableWebsite.Name} Base URL: {availableWebsite.Url}")
            : "\nNo websites available.";

        await Context.Message.Channel.SendMessageAsync(message);
    }
}