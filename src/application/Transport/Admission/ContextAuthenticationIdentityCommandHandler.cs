using ChristianSchulz.MultitenancyMonolith.Application.Admission.Commands;
using ChristianSchulz.MultitenancyMonolith.Configuration;
using Microsoft.AspNetCore.Authentication.BearerToken;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

internal sealed class ContextAuthenticationIdentityCommandHandler : IContextAuthenticationIdentityCommandHandler
{
    private readonly IAuthenticationIdentityManager _authenticationIdentityManager;
    private readonly IAuthenticationIdentityVerificationManager _identityVerificationManager;
    private readonly IAllowedClientsProvider _allowedClientsProvider;

    public ContextAuthenticationIdentityCommandHandler(
        IAuthenticationIdentityManager authenticationIdentityManager,
        IAuthenticationIdentityVerificationManager identityVerificationManager,
        IAllowedClientsProvider allowedClientsProvider)
    {
        _authenticationIdentityManager = authenticationIdentityManager;
        _identityVerificationManager = identityVerificationManager;
        _allowedClientsProvider = allowedClientsProvider;
    }

    public async Task<ClaimsPrincipal> AuthAsync(ContextAuthenticationIdentityAuthCommand command)
    {
        var clientName = command.ClientName;
        if (_allowedClientsProvider.Get().All(x => x.Service != clientName))
        {
            TransportException.ThrowSecurityViolation($"Client name '{clientName}' is not allowed to sign in");
        }

        var authenticationIdentityExists = await _authenticationIdentityManager.ExistsAsync(command.AuthenticationIdentity, command.Secret);
        if (!authenticationIdentityExists)
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

        var claimsIdentity = new ClaimsIdentity(claims, BearerTokenDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        return claimsPrincipal;
    }

    public void Verify() { }
}