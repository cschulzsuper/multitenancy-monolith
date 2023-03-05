using ChristianSchulz.MultitenancyMonolith.Application.Admission.Commands;
using ChristianSchulz.MultitenancyMonolith.Configuration;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

internal sealed class AuthenticationIdentityCommandHandler : IAuthenticationIdentityCommandHandler
{
    private readonly IAuthenticationIdentityManager _authenticationIdentityManager;
    private readonly IAuthenticationIdentityVerificationManager _identityVerificationManager;
    private readonly IAllowedClientsProvider _allowedClientsProvider;

    public AuthenticationIdentityCommandHandler(
        IAuthenticationIdentityManager authenticationIdentityManager,
        IAuthenticationIdentityVerificationManager identityVerificationManager,
        IAllowedClientsProvider allowedClientsProvider)
    {
        _authenticationIdentityManager = authenticationIdentityManager;
        _identityVerificationManager = identityVerificationManager;
        _allowedClientsProvider = allowedClientsProvider;
    }

    public async Task<ClaimsIdentity> AuthAsync(AuthenticationIdentityAuthCommand command)
    {
        var client = command.Client;

        if (_allowedClientsProvider.Get().All(x => x.UniqueName != client))
        {
            TransportException.ThrowSecurityViolation($"Client '{client}' is not allowed to sign in");
        }

        var found = await _authenticationIdentityManager.ExistsAsync(command.Identity, command.Secret);

        if (!found)
        {
            TransportException.ThrowSecurityViolation($"Could not match authentication identity '{command.Identity}' against secret");
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