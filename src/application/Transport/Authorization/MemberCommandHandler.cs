using ChristianSchulz.MultitenancyMonolith.Application.Authorization.Commands;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Claims;
using System;
using System.Linq;
using System.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authorization;

internal sealed class MemberCommandHandler : IMemberCommandHandler
{
    private readonly IMembershipManager _membershipManager;
    private readonly IMembershipVerficationManager _membershipVerficationManager;
    private readonly ClaimsPrincipal _user;

    private readonly string[] _allowedClients = { "swagger", "endpoint-tests" };

    private readonly static object _signInLock = new();

    public MemberCommandHandler(
        IMembershipManager membershipManager,
        IMembershipVerficationManager membershipVerficationManager,
        ClaimsPrincipal user)
    {
        _membershipManager = membershipManager;
        _membershipVerficationManager = membershipVerficationManager;
        _user = user;
    }

    public ClaimsIdentity SignIn(string group, string member, MemberSignInCommand command)
    {
        var client = command.Client;

        if (!_allowedClients.Contains(command.Client))
        {
            TransportException.ThrowSecurityViolation($"Client '{client}' is not allowed to sign in");
        }

        if (command.Client != _user.GetClaimOrDefault("client"))
        {
            TransportException.ThrowSecurityViolation($"Not allowed to switch to client '{client}'");
        }


        lock (_signInLock)
        {
            var identity = _user.GetClaim("identity");

            var found = _membershipManager
                .GetQueryable()
                .Any(x =>
                    x.Group == group &&
                    x.Member == member &&
                    x.Identity == identity);

            if (!found)
            {
                TransportException.ThrowSecurityViolation($"Member '{member}' does not exist in group '{group}'");
            }

            var verfication = Guid.NewGuid().ToByteArray();

            var verfifytionKey = new MembershipVerficationKey
            {
                Client = client,
                Group = group,
                Member = member
            };

            _membershipVerficationManager.Set(verfifytionKey, verfication);

            var verficationnValue = Convert.ToBase64String(verfication);

            var claims = new Claim[]
            {
            new Claim("client", client),
            new Claim("identity", identity),
            new Claim("group", group),
            new Claim("member", member),
            new Claim("verification", verficationnValue, ClaimValueTypes.Base64Binary)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Badge");

            return claimsIdentity;
        }
    }

    public void Verify() { }
}