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
        var clientName = command.ClientName;

        if (_allowedClientsProvider.Get().All(x => x.UniqueName != clientName))
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
        var authenticationIdentityFound = accountMember.AuthenticationIdentities.Any(x => x.UniqueName == authenticationIdentity);

        if (!authenticationIdentityFound)
        {
            TransportException.ThrowSecurityViolation($"Can not sign in as account member '{command.AccountMember}' in account group '{command.AccountGroup}'.");
        }

        var verification = Guid.NewGuid().ToByteArray();

        var verificationKey = new AccountMemberVerificationKey
        {
            ClientName = clientName,
            AuthenticationIdentity = authenticationIdentity,
            AccountGroup = command.AccountGroup,
            AccountMember = command.AccountMember
        };

        _memberVerificationManager.Set(verificationKey, verification);

        var verificationnValue = Convert.ToBase64String(verification);

        var claims = new Claim[]
        {
            new Claim("type", "member"),
            new Claim("client", clientName),
            new Claim("identity", authenticationIdentity),
            new Claim("group", command.AccountGroup),
            new Claim("member", command.AccountMember),
            new Claim("verification", verificationnValue, ClaimValueTypes.Base64Binary)
        };

        var claimsIdentity = new ClaimsIdentity(claims, "Badge");

        return claimsIdentity;

    }

    public void Verify() { }
}