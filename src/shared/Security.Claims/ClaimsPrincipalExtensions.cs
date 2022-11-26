using System.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Security.Claims;

public static class ClaimsPrincipalExtensions
{
    public static string GetClaim(this ClaimsPrincipal principal, string claim)
        => GetClaimOrDefault(principal, claim)
            ?? throw new ClaimsException($"Could not find '{claim}' claim.");

    public static string? GetClaimOrDefault(this ClaimsPrincipal principal, string claim)
        => principal.Claims.SingleOrDefault(x => x.Type == claim)?.Value;
}