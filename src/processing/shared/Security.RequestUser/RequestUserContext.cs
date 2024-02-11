using System.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Security.RequestUser;

public sealed class RequestUserContext
{
    public RequestUserContext()
    {
        User = new ClaimsPrincipal();
    }

    public ClaimsPrincipal User { get; internal set; }
}