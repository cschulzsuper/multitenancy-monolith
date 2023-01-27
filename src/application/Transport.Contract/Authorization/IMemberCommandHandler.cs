using ChristianSchulz.MultitenancyMonolith.Application.Authorization.Commands;
using System.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authorization;

public interface IMemberCommandHandler
{
    ClaimsIdentity SignIn(string group, string member, MemberCommand command);
    void Verify();
}