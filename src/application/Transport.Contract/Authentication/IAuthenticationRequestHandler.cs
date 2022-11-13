using ChristianSchulz.MultitenancyMonolith.Application.Authentication.Request;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

public interface IAuthenticationRequestHandler
{
    string SignIn(string identity, SignInRequest request);
}
