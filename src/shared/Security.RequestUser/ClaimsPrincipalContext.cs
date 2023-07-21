using System.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Security.RequestUser;

public sealed class ClaimsPrincipalContext
{
    public ClaimsPrincipalContext()
    {
        User = new ClaimsPrincipal();
    }

    public ClaimsPrincipal User { get; internal set; }
}