using System.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Security.Claims;

public class ClaimsPrincipalContext
{
    public ClaimsPrincipalContext()
    {
        User = new ClaimsPrincipal();
    }

    public ClaimsPrincipal User { get; internal set; }
}