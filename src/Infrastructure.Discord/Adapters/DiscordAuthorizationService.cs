using Application.Ports.Authorization;
using Infrastructure.Discord.Settings;
using Microsoft.Extensions.Options;

namespace Infrastructure.Discord.Adapters;

public class DiscordAuthorizationService : IAuthorizationService
{
    private readonly AdminSettings _adminSettings;

    public DiscordAuthorizationService(IOptions<AdminSettings> adminSettings)
    {
        _adminSettings = adminSettings.Value;
    }

    public bool IsAdmin(string externalIdentifier)
    {
        return _adminSettings.DiscordAdminIds.Contains(externalIdentifier);
    }
}