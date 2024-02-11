using System.Linq;
using System.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Security.Claims;

public static class ClaimsPrincipalExtensions
{
    public static string GetClaim(this ClaimsPrincipal principal, string claim)
    {
        var claimValue = GetClaimOrDefault(principal, claim);

        if (claimValue == null)
        {
            ClaimsException.ThrowClaimNotFound(claim);
        }

        return claimValue;
    }

    public static string? GetClaimOrDefault(this ClaimsPrincipal principal, string claim)
        => principal.Claims.SingleOrDefault(x => x.Type == claim)?.Value;
}