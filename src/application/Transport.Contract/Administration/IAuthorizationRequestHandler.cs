using System.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public interface IAuthorizationRequestHandler
{
    ClaimsIdentity TakeUp(ClaimsPrincipal user, string group, string uniqueName);
    void Verify();
}
