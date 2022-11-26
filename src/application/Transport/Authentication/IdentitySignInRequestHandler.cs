using ChristianSchulz.MultitenancyMonolith.Application.Authentication.Requests;
using System;
using System.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

internal sealed class IdentitySignInRequestHandler : IIdentitySignInRequestHandler
{
    private readonly IIdentityManager _identityManager;
    private readonly IIdentityVerficationManager _identityVerficationManager;

    private readonly static object _signInLock = new();

    public IdentitySignInRequestHandler(
        IIdentityManager identityManager,
        IIdentityVerficationManager identityVerficationManager)
    {
        _identityManager = identityManager;
        _identityVerficationManager = identityVerficationManager;
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

            var identityVerification = Guid
                .NewGuid()
                .ToByteArray();

            _identityVerficationManager.Set(identity.UniqueName, identityVerification);

            var identityVerificationString = Convert.ToBase64String(identityVerification);

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