using ChristianSchulz.MultitenancyMonolith.Application.Authorization.Commands;
using ChristianSchulz.MultitenancyMonolith.Configuration;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Claims;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authorization;

internal sealed class MemberCommandHandler : IMemberCommandHandler
{
    private readonly IMemberManager _memberManager;
    private readonly IMemberVerificationManager _memberVerificationManager;
    private readonly IAllowedClientsProvider _allowedClientsProvider;
    private readonly ClaimsPrincipal _user;

    public MemberCommandHandler(
        IMemberManager memberManager,
        IMemberVerificationManager memberVerificationManager,
        IAllowedClientsProvider allowedClientsProvider,
        ClaimsPrincipal user)
    {
        _memberManager = memberManager;
        _memberVerificationManager = memberVerificationManager;
        _allowedClientsProvider = allowedClientsProvider;
        _user = user;
    }

    public async ValueTask<ClaimsIdentity> AuthAsync(MemberAuthCommand command)
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

        var member = await _memberManager.GetOrDefaultAsync(command.Member);
        if (member == null)
        {
            TransportException.ThrowSecurityViolation($"Member '{command.Member}' is not in group '{command.Group}'.");
        }

        var identity = _user.GetClaim("identity");
        var identityFound = member.Identities.Any(x => x.UniqueName == identity);

        if (!identityFound)
        {
            TransportException.ThrowSecurityViolation($"Can not sign in as member '{command.Member}' in group '{command.Group}'.");
        }

        var verification = Guid.NewGuid().ToByteArray();

        var verificationKey = new MemberVerificationKey
        {
            Client = client,
            Identity = identity,
            Group = command.Group,
            Member = command.Member
        };

        _memberVerificationManager.Set(verificationKey, verification);

        var verificationnValue = Convert.ToBase64String(verification);

        var claims = new Claim[]
        {
            new Claim("type", "member"),
            new Claim("client", client),
            new Claim("identity", identity),
            new Claim("group", command.Group),
            new Claim("member", command.Member),
            new Claim("verification", verificationnValue, ClaimValueTypes.Base64Binary)
        };

        var claimsIdentity = new ClaimsIdentity(claims, "Badge");

        return claimsIdentity;

    }

    public void Verify() { }
}