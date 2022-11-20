using ChristianSchulz.MultitenancyMonolith.Application.Authentication.Request;
using System;
using System.Security.Claims;

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

    public ClaimsIdentity SignIn(string uniqueName, SignInRequest request)
    {
        lock (_signInLock)
        {
            var identity = _identityManager.Get(uniqueName);

            var valid = request.Secret == identity.Secret;

            if (!valid)
            {
                throw new TransportException($"The secret does not match.");
            }

            identity.Verification = Guid
                .NewGuid()
                .ToByteArray();

            var identityVerificationString = Convert.ToBase64String(identity.Verification);

            var claims = new Claim[]
            {
                new Claim("Identity", identity.UniqueName),
                new Claim("Verification", identityVerificationString, ClaimValueTypes.Base64Binary)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Badge");

            return claimsIdentity;
        }
    }

    public void Verify() { }
}