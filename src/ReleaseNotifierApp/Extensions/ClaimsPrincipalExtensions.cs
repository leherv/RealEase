using System.Security.Claims;

namespace ReleaseNotifierApp.Extensions;

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
}