using Discord.Commands;

namespace Infrastructure.Discord.Extensions;

internal static class SocketCommandContextExtensions
{
    internal static ulong GetUserId(this SocketCommandContext context)
    {
        return context.User.Id;
    }
}