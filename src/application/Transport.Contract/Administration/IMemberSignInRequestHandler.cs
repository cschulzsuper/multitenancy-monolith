using System.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public interface IMemberSignInRequestHandler
{
    ClaimsIdentity SignIn(string group, string uniqueName);
    void Verify();
}
