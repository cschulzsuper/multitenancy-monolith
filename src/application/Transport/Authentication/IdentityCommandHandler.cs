using ChristianSchulz.MultitenancyMonolith.Application.Authentication.Commands;
using System;
using System.Linq;
using System.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

internal sealed class IdentityCommandHandler : IIdentityCommandHandler
{
    private readonly IIdentityManager _identityManager;
    private readonly IIdentityVerficationManager _identityVerficationManager;

    private readonly string[] _allowedClients = { "swagger", "endpoint-tests" };

    private readonly static object _signInLock = new();

    public IdentityCommandHandler(
        IIdentityManager identityManager,
        IIdentityVerficationManager identityVerficationManager)
    {
        _identityManager = identityManager;
        _identityVerficationManager = identityVerficationManager;
    }

    public ClaimsIdentity SignIn(string identity, IdentitySignInCommand command)
    {
        var client = command.Client;

        if (!_allowedClients.Contains(command.Client))
        {
            TransportException.ThrowSecurityViolation($"Client '{client}' is not allowed to sign in");
        }

        lock (_signInLock)
        {
            var found = _identityManager.GetAll()
                .Any(x =>
                    x.UniqueName == identity &&
                    x.Secret == command.Secret);

            if (!found)
            {
                TransportException.ThrowSecurityViolation($"Could not match identity '{identity}' against secret");
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
            new Claim("client", client),
            new Claim("identity", identity),
            new Claim("verification", verificationValue, ClaimValueTypes.Base64Binary)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Badge");

            return claimsIdentity;
        }
    }

    public void Verify() { }
}