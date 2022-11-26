using ChristianSchulz.MultitenancyMonolith.Application.Authentication.Requests;
using System.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

public interface IIdentitySignInRequestHandler
{
    ClaimsIdentity SignIn(string identity, SignInRequest request);
    void Verify();
}
