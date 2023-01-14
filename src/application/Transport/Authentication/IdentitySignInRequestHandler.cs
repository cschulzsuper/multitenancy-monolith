using ChristianSchulz.MultitenancyMonolith.Application.Authentication.Requests;
using System;
using System.Linq;
using System.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

internal sealed class IdentitySignInRequestHandler : IIdentitySignInRequestHandler
{
    private readonly IIdentityManager _identityManager;
    private readonly IIdentityVerficationManager _identityVerficationManager;

    private readonly string[] _allowedClients = { "swagger", "endpoint-tests" };

    private readonly static object _signInLock = new();

    public IdentitySignInRequestHandler(
        IIdentityManager identityManager,
        IIdentityVerficationManager identityVerficationManager)
    {
        _identityManager = identityManager;
        _identityVerficationManager = identityVerficationManager;
    }

    public ClaimsIdentity SignIn(string identity, IdentitySignInRequest request)
    {
        var client = request.Client;

        if (!_allowedClients.Contains(request.Client))
        {
            throw new TransportException($"Client '{client}' is not allowed to sign in");
        }

        lock (_signInLock)
        {
            var found = _identityManager.GetAll()
                .Any(x => 
                    x.UniqueName == identity && 
                    x.Secret == request.Secret);

            if (!found)
            {
                throw new TransportException($"Could match identity '{identity}' against secret '{request.Secret}'");
            }

            var verification = Guid.NewGuid().ToByteArray();

            var verfifytionKey = new IdentityVerficationKey
            {
                Client = client,
                Identity = identity
            };

            _identityVerficationManager.Set(verfifytionKey, verification);

            var verificationValue = Convert.ToBase64String(verification);

            var claims = new Claim[]
            {
                new Claim("Client", client),
                new Claim("Identity", identity),
                new Claim("Verification", verificationValue, ClaimValueTypes.Base64Binary)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Badge");

            return claimsIdentity;
        }
    }

    public void Verify() { }
}