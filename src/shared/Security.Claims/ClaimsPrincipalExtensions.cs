using System.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Security.Claims;

public static class ClaimsPrincipalExtensions
{
    public static string? GetIdentityOrDefault(this ClaimsPrincipal principal)
        => principal.Claims.SingleOrDefault(x => x.Type == "Identity")?.Value;

    public static string? GetMemberOrDefault(this ClaimsPrincipal principal)
        => principal.Claims.SingleOrDefault(x => x.Type == "Member")?.Value;

    public static string? GetGroupOrDefault(this ClaimsPrincipal principal)
        => principal.Claims.SingleOrDefault(x => x.Type == "Group")?.Value;
}