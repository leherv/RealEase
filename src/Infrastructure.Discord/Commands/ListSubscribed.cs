using Application.UseCases.Base;
using Application.UseCases.Subscriber.QueryMediaSubscriptions;
using Discord.Commands;

namespace Infrastructure.Discord.Commands;

public class ListSubscribed : ModuleBase<SocketCommandContext>
{
    private readonly IQueryDispatcher _queryDispatcher;

    public ListSubscribed(IQueryDispatcher queryDispatcher)
    {
        _queryDispatcher = queryDispatcher;
    }

    [Command("listSubscribed")]
    [Alias("ls")]
    public async Task ListSubscribedHandler()
    {
        var mediaSubscriptions =
            await _queryDispatcher.Dispatch<MediaSubscriptionsQuery, MediaSubscriptions>(
                new MediaSubscriptionsQuery(Context.User.Id.ToString()));

        var message = mediaSubscriptions.SubscribedToMediaNames.Any()
            ? $"Subscribed To:\n{string.Join("\n", mediaSubscriptions.SubscribedToMediaNames)}"
            : "No subscriptions yet";

        await Context.Message.Channel.SendMessageAsync(message);
    }
}