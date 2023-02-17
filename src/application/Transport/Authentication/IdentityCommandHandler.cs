using ChristianSchulz.MultitenancyMonolith.Application.Authentication.Commands;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

internal sealed class IdentityCommandHandler : IIdentityCommandHandler
{
    private readonly IIdentityManager _identityManager;
    private readonly IIdentityVerificationManager _identityVerificationManager;

    private readonly string[] _allowedClients = { "swagger", "security-tests" };

    public IdentityCommandHandler(
        IIdentityManager identityManager,
        IIdentityVerificationManager identityVerificationManager)
    {
        _identityManager = identityManager;
        _identityVerificationManager = identityVerificationManager;
    }

    public async ValueTask<ClaimsIdentity> AuthAsync(IdentityAuthCommand command)
    {
        var client = command.Client;

        if (!_allowedClients.Contains(command.Client))
        {
            TransportException.ThrowSecurityViolation($"Client '{client}' is not allowed to sign in");
        }

        var found = await _identityManager.ExistsAsync(command.Identity, command.Secret);

        if (!found)
        {
            TransportException.ThrowSecurityViolation($"Could not match identity '{command.Identity}' against secret");
        }

        var verification = Guid.NewGuid().ToByteArray();

        var verificationKey = new IdentityVerificationKey
        {
            Client = client,
            Identity = command.Identity
        };

        _identityVerificationManager.Set(verificationKey, verification);

        var verificationValue = Convert.ToBase64String(verification);

        var claims = new Claim[]
        {
            new Claim("type", "identity"),
            new Claim("client", client),
            new Claim("identity", command.Identity),
            new Claim("verification", verificationValue, ClaimValueTypes.Base64Binary)
        };

        var claimsIdentity = new ClaimsIdentity(claims, "Badge");

        return claimsIdentity;
    }

    public void Verify() { }
}