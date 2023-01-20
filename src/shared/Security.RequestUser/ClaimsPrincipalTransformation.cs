using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Security.RequestUser;

internal sealed class ClaimsPrincipalTransformation : IClaimsTransformation
{
    private readonly ClaimsPrincipalContext _claimsPrincipalContext;

    public ClaimsPrincipalTransformation(ClaimsPrincipalContext claimsPrincipalContext)
    {
        _claimsPrincipalContext = claimsPrincipalContext;
    }

    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        _claimsPrincipalContext.User = principal;

        return Task.FromResult(principal);
    }
}