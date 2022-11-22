using ChristianSchulz.MultitenancyMonolith.Application.Authentication.Requests;
using System.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

public interface IAuthenticationRequestHandler
{
    ClaimsIdentity SignIn(string uniqueName, SignInRequest request);
    void Verify();
}
