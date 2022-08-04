using Application.UseCases.Base;
using Application.UseCases.Subscriber.QueryMediaSubscriptions;
using Discord.Commands;

namespace Infrastructure.Discord.Commands;

public class ListSubscribed : ModuleBase<SocketCommandContext>
{
    private readonly IQueryDispatcher _queryDispatcher;

    internal ListSubscribed(IQueryDispatcher queryDispatcher)
    {
        _queryDispatcher = queryDispatcher;
    }

    [Command("listSubscribed")]
    [Alias("ls")]
    internal async Task ListSubscribedHandler()
    {
        var mediaSubscriptions =
            await _queryDispatcher.Dispatch<MediaSubscriptionsQuery, MediaSubscriptions>(
                new MediaSubscriptionsQuery(Context.User.Id.ToString()));

        var message = mediaSubscriptions.SubscribedToMedia.Any()
            ? $"Subscribed To:\n{string.Join("\n", mediaSubscriptions.SubscribedToMedia.Select(mediaSubscriptionInfo => mediaSubscriptionInfo.MediaName))}"
            : "No subscriptions yet";

        await Context.Message.Channel.SendMessageAsync(message);
    }
}