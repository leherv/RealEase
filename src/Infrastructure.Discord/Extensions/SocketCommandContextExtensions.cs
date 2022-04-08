using Discord.Commands;

namespace Infrastructure.Discord.Extensions;

public static class SocketCommandContextExtensions
{
    public static ulong GetUserId(this SocketCommandContext context)
    {
        return context.User.Id;
    }
}