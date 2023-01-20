using ChristianSchulz.MultitenancyMonolith.Application.Administration.Requests;
using System.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public interface IMemberSignInRequestHandler
{
    ClaimsIdentity SignIn(string group, string member, MemberSignInRequest request);
    void Verify();
}