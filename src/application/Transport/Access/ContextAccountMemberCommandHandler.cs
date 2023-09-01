using ChristianSchulz.MultitenancyMonolith.Application.Access.Commands;
using ChristianSchulz.MultitenancyMonolith.Configuration;
using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Claims;
using Microsoft.AspNetCore.Authentication.BearerToken;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

internal sealed class ContextAccountMemberCommandHandler : IContextAccountMemberCommandHandler
{
    private readonly IAccountMemberManager _accountMemberManager;
    private readonly AllowedClient[] _allowedClients;
    private readonly ClaimsPrincipal _user;

    public ContextAccountMemberCommandHandler(
        IAccountMemberManager accountMemberManager,
        IConfigurationProxyProvider configurationProxyProvider,
        ClaimsPrincipal user)
    {
        _accountMemberManager = accountMemberManager;
        _allowedClients = configurationProxyProvider.GetAllowedClients();
        _user = user;
    }

    public async Task<object> AuthAsync(ContextAccountMemberAuthCommand command)
    {
        var clientName = command.ClientName;

        if (_allowedClients.All(x => x.Service != clientName))
        {
            TransportException.ThrowSecurityViolation($"Client name '{clientName}' is not allowed to sign in.");
        }

        if (command.ClientName != _user.GetClaimOrDefault("client"))
        {
            TransportException.ThrowSecurityViolation($"Not allowed to switch to client name '{clientName}'.");
        }

        var accountMember = await _accountMemberManager.GetOrDefaultAsync(command.AccountMember);
        if (accountMember == null)
        {
            TransportException.ThrowSecurityViolation($"Account member '{command.AccountMember}' is not in group '{command.AccountGroup}'.");
        }

        var authenticationIdentity = _user.GetClaim("identity");
        var authenticationIdentityPermitted = accountMember.AuthenticationIdentities.Any(x => x.UniqueName == authenticationIdentity);
        if (!authenticationIdentityPermitted)
        {
            TransportException.ThrowSecurityViolation($"Can not sign in as account member '{command.AccountMember}' in account group '{command.AccountGroup}'.");
        }

        var claims = new Claim[]
        {
            new Claim("type", "member"),
            new Claim("client", clientName),
            new Claim("identity", authenticationIdentity),
            new Claim("group", command.AccountGroup),
            new Claim("member", command.AccountMember),
        };

        var claimsIdentity = new ClaimsIdentity(claims, BearerTokenDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        return claimsPrincipal;
    }

    public Task VerifyAsync() 
        => Task.CompletedTask;
}