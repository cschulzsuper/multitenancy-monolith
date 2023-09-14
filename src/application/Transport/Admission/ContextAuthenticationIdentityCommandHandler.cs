using ChristianSchulz.MultitenancyMonolith.Application.Admission.Commands;
using ChristianSchulz.MultitenancyMonolith.Configuration;
using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Admission.ConcreteValidators;
using Microsoft.AspNetCore.Authentication.BearerToken;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

internal sealed class ContextAuthenticationIdentityCommandHandler : IContextAuthenticationIdentityCommandHandler
{
    private readonly IAuthenticationIdentityManager _authenticationIdentityManager;
    private readonly IAuthenticationIdentityAuthenticationMethodManager _authenticationIdentityAuthenticationMethodManager;
    private readonly AllowedClient[] _allowedClients;

    public ContextAuthenticationIdentityCommandHandler(
        IAuthenticationIdentityManager authenticationIdentityManager,
        IAuthenticationIdentityAuthenticationMethodManager authenticationIdentityAuthenticationMethodManager,
        IConfigurationProxyProvider configurationProxyProvider)
    {
        _authenticationIdentityManager = authenticationIdentityManager;
        _authenticationIdentityAuthenticationMethodManager = authenticationIdentityAuthenticationMethodManager;
        _allowedClients = configurationProxyProvider.GetAllowedClients();
    }

    public async Task<object> AuthAsync(ContextAuthenticationIdentityAuthCommand command)
    {
        var clientName = command.ClientName;
        if (_allowedClients.All(x => x.Service != clientName))
        {
            TransportException.ThrowSecurityViolation($"Client name '{clientName}' is not allowed to sign in");
        }

        var authenticationMethod = command.AuthenticationMethod ?? AuthenticationMethods.Secret;
        var authenticationMethodExists = await _authenticationIdentityAuthenticationMethodManager.ExistsAsync(command.AuthenticationIdentity, command.ClientName, authenticationMethod);
        
        switch (authenticationMethod)
        {
            case AuthenticationMethods.Anonymouse when authenticationMethodExists:
                break;

            case AuthenticationMethods.Secret:
                var secretProvided = command.Secret != null;
                if (!secretProvided)
                {
                    TransportException.ThrowSecurityViolation($"Secret is required for authentication method '{AuthenticationMethods.Secret}'");
                }

                var authenticationIdentityExists = await _authenticationIdentityManager.ExistsAsync(command.AuthenticationIdentity, command.Secret!);
                if (!authenticationIdentityExists)
                {
                    TransportException.ThrowSecurityViolation($"Could not match authentication identity '{command.AuthenticationIdentity}' against secret");
                }
                break;

            default:
                TransportException.ThrowSecurityViolation($"Unkown authentication method '{authenticationMethod}'");
                break;
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