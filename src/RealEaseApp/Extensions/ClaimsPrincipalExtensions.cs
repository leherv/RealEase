using System.Security.Claims;

namespace RealEaseApp.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static bool IsAuthenticated(this ClaimsPrincipal principal)
    {
        return principal.Identity?.IsAuthenticated ?? false;
    }
    
    public static string? GetUserDiscriminator(this ClaimsPrincipal principal)
    {
        return principal.Claims.SingleOrDefault(c => c.Type == "urn:discord:user:discriminator")?.Value;
    }
    
    public static string? GetExternalIdentifier(this ClaimsPrincipal principal)
    {
        return principal.Claims.SingleOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
    }
    
    public static string? GetName(this ClaimsPrincipal principal)
    {
        return principal.Claims.SingleOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
    }
    
    public static bool BotAdded(this ClaimsPrincipal principal)
    {
        var botAddedString = principal.Claims.SingleOrDefault(c => c.Type == "botAdded")?.Value;
        return bool.TryParse(botAddedString, out var botAdded) && botAdded;
    }
    
    public static bool IsAdmin(this ClaimsPrincipal principal)
    {
        var isAdminString = principal.Claims.SingleOrDefault(c => c.Type == "isAdmin")?.Value;
        return bool.TryParse(isAdminString, out var isAdmin) && isAdmin;
    }
}