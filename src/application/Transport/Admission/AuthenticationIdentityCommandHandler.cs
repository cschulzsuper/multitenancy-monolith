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
        var clientName = command.ClientName;

        if (_allowedClientsProvider.Get().All(x => x.UniqueName != clientName))
        {
            TransportException.ThrowSecurityViolation($"Client name '{clientName}' is not allowed to sign in");
        }

        var found = await _authenticationIdentityManager.ExistsAsync(command.AuthenticationIdentity, command.Secret);

        if (!found)
        {
            TransportException.ThrowSecurityViolation($"Could not match authentication identity '{command.AuthenticationIdentity}' against secret");
        }

        var verification = Guid.NewGuid().ToByteArray();

        var verificationKey = new AuthenticationIdentityVerificationKey
        {
            ClientName = clientName,
            AuthenticationIdentity = command.AuthenticationIdentity
        };

        _identityVerificationManager.Set(verificationKey, verification);

        var verificationValue = Convert.ToBase64String(verification);

        var claims = new Claim[]
        {
            new Claim("type", "identity"),
            new Claim("client", clientName),
            new Claim("identity", command.AuthenticationIdentity),
            new Claim("verification", verificationValue, ClaimValueTypes.Base64Binary)
        };

        var claimsIdentity = new ClaimsIdentity(claims, "Badge");

        return claimsIdentity;
    }

    public void Verify() { }
}