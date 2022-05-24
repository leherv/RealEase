using Application.UseCases.Base;
using Application.UseCases.Media.QueryAvailableMedia;
using Discord.Commands;

namespace Infrastructure.Discord.Commands;

public class ListAvailable : ModuleBase<SocketCommandContext>
{
    private readonly IQueryDispatcher _queryDispatcher;

    internal ListAvailable(IQueryDispatcher queryDispatcher)
    {
        _queryDispatcher = queryDispatcher;
    }

    [Command("listAvailable")]
    [Alias("l")]
    internal async Task ListAvailableHandler()
    {
        var availableMedia =
            await _queryDispatcher.Dispatch<AvailableMediaQuery, AvailableMedia>(new AvailableMediaQuery());

        var message = availableMedia.MediaNames.Any()
            ? "Available Media: \n" +
              $"{string.Join("\n", availableMedia.MediaNames)}"
            : "No media available.";

        await Context.Message.Channel.SendMessageAsync(message);
    }
}