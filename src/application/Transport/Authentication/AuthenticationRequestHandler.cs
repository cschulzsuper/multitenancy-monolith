using ChristianSchulz.MultitenancyMonolith.Application.Authentication.Request;
using System;
using System.Linq;
using System.Text;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

public class AuthenticationRequestHandler : IAuthenticationRequestHandler
{
    private readonly IIdentityManager _identityManager;

    private readonly static object _signInLock = new();

    public AuthenticationRequestHandler(
        IIdentityManager identityManager)
    {
        _identityManager = identityManager;
    }

    public string SignIn(string uniqueName, SignInRequest request)
    {
        lock (_signInLock)
        {
            var user = _identityManager.Get(uniqueName);

            var valid = request.Secret == user.Secret;

            if (!valid)
            {
                throw new TransportException($"The secret does not match.");
            }

            user.Verification = Guid
                .NewGuid()
                .ToByteArray();

            var badge = CreateBadge(user);

            return badge;
        }
    }

    private static string CreateBadge(Identity identity)
    {
        var uniqueNameBytes = Encoding.UTF8.GetBytes(identity.UniqueName);

        var badgeBytes = Enumerable
            .Concat(identity.Verification, uniqueNameBytes)
            .ToArray();

        return Convert.ToBase64String(badgeBytes);
    }
}