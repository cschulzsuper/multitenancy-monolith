using ChristianSchulz.MultitenancyMonolith.Application.Admission.Commands;
using ChristianSchulz.MultitenancyMonolith.Configuration;
using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;
using Microsoft.AspNetCore.Authentication.BearerToken;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

internal sealed class ContextAuthenticationIdentityCommandHandler : IContextAuthenticationIdentityCommandHandler
{
    private readonly IAuthenticationIdentityManager _authenticationIdentityManager;
    private readonly AllowedClient[] _allowedClients;

    public ContextAuthenticationIdentityCommandHandler(
        IAuthenticationIdentityManager authenticationIdentityManager,
        IConfigurationProxyProvider configurationProxyProvider)
    {
        _authenticationIdentityManager = authenticationIdentityManager;
        _allowedClients = configurationProxyProvider.GetAllowedClients();
    }

    public async Task<object> AuthAsync(ContextAuthenticationIdentityAuthCommand command)
    {
        var clientName = command.ClientName;
        if (_allowedClients.All(x => x.Service != clientName))
        {
            TransportException.ThrowSecurityViolation($"Client name '{clientName}' is not allowed to sign in");
        }

        var authenticationIdentityExists = await _authenticationIdentityManager.ExistsAsync(command.AuthenticationIdentity, command.Secret);
        if (!authenticationIdentityExists)
        {
            TransportException.ThrowSecurityViolation($"Could not match authentication identity '{command.AuthenticationIdentity}' against secret");
        }

        var claims = new Claim[]
        {
            new Claim("type", "identity"),
            new Claim("client", command.ClientName),
            new Claim("identity", command.AuthenticationIdentity)
        };

        var claimsIdentity = new ClaimsIdentity(claims, BearerTokenDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        return claimsPrincipal;
    }

    public Task VerifyAsync()
        => Task.CompletedTask;
}