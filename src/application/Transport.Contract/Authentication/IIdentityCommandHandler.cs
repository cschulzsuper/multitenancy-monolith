using ChristianSchulz.MultitenancyMonolith.Application.Authentication.Commands;
using System.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

public interface IIdentityCommandHandler
{
    ClaimsIdentity SignIn(string identity, IdentitySignInCommand command);
    void Verify();
}