using ChristianSchulz.MultitenancyMonolith.Application.Access.Commands;
using ChristianSchulz.MultitenancyMonolith.Configuration;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Claims;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

internal sealed class AccountMemberCommandHandler : IAccountMemberCommandHandler
{
    private readonly IAccountMemberManager _accountMemberManager;
    private readonly IAccountMemberVerificationManager _memberVerificationManager;
    private readonly IAllowedClientsProvider _allowedClientsProvider;
    private readonly ClaimsPrincipal _user;

    public AccountMemberCommandHandler(
        IAccountMemberManager accountMemberManager,
        IAccountMemberVerificationManager accountMemberVerificationManager,
        IAllowedClientsProvider allowedClientsProvider,
        ClaimsPrincipal user)
    {
        _accountMemberManager = accountMemberManager;
        _memberVerificationManager = accountMemberVerificationManager;
        _allowedClientsProvider = allowedClientsProvider;
        _user = user;
    }

    public async Task<ClaimsIdentity> AuthAsync(AccountMemberAuthCommand command)
    {
        var client = command.Client;

        if (_allowedClientsProvider.Get().All(x => x.UniqueName != client))
        {
            TransportException.ThrowSecurityViolation($"Client '{client}' is not allowed to sign in.");
        }

        if (command.Client != _user.GetClaimOrDefault("client"))
        {
            TransportException.ThrowSecurityViolation($"Not allowed to switch to client '{client}'.");
        }

        var accountMember = await _accountMemberManager.GetOrDefaultAsync(command.Member);
        if (accountMember == null)
        {
            TransportException.ThrowSecurityViolation($"Account member '{command.Member}' is not in group '{command.Group}'.");
        }

        var authenticationIdentity = _user.GetClaim("identity");
        var authenticationIdentityFound = accountMember.AuthenticationIdentities.Any(x => x.UniqueName == authenticationIdentity);

        if (!authenticationIdentityFound)
        {
            TransportException.ThrowSecurityViolation($"Can not sign in as account member '{command.Member}' in account group '{command.Group}'.");
        }

        var verification = Guid.NewGuid().ToByteArray();

        var verificationKey = new AccountMemberVerificationKey
        {
            Client = client,
            Identity = authenticationIdentity,
            Group = command.Group,
            Member = command.Member
        };

        _memberVerificationManager.Set(verificationKey, verification);

        var verificationnValue = Convert.ToBase64String(verification);

        var claims = new Claim[]
        {
            new Claim("type", "member"),
            new Claim("client", client),
            new Claim("identity", authenticationIdentity),
            new Claim("group", command.Group),
            new Claim("member", command.Member),
            new Claim("verification", verificationnValue, ClaimValueTypes.Base64Binary)
        };

        var claimsIdentity = new ClaimsIdentity(claims, "Badge");

        return claimsIdentity;

    }

    public void Verify() { }
}