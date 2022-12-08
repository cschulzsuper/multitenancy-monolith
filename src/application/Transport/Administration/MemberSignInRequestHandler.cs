using ChristianSchulz.MultitenancyMonolith.Shared.Security.Claims;
using System;
using System.Linq;
using System.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal sealed class MemberSignInRequestHandler : IMemberSignInRequestHandler
{
    private readonly IMembershipManager _membershipManager;
    private readonly IMembershipVerficationManager _membershipVerficationManager;
    private readonly ClaimsPrincipal _user;

    private readonly static object _signInLock = new();

    public MemberSignInRequestHandler(
        IMembershipManager membershipManager,
        IMembershipVerficationManager membershipVerficationManager,
        ClaimsPrincipal user)
    {
        _membershipManager = membershipManager;
        _membershipVerficationManager = membershipVerficationManager;
        _user = user;
    }

    public ClaimsIdentity SignIn(string group, string member)
    {
        lock (_signInLock)
        {
            var identity = _user.GetClaim("Identity");

            var found = _membershipManager
                .GetAll()
                .Any(x =>
                    x.Group == group &&
                    x.Member == member &&
                    x.Identity == identity);

            if (!found)
            {
                throw new TransportException($"Member '{member}' does not exist in group '{group}'");
            }

            var verfication = Guid.NewGuid().ToByteArray();

            _membershipVerficationManager.Set(group, member, verfication);

            var verficationnValue = Convert.ToBase64String(verfication);

            var claims = new Claim[]
            {
                new Claim("Identity", identity),
                new Claim("Group", group),
                new Claim("Member", member),
                new Claim("Verification", verficationnValue, ClaimValueTypes.Base64Binary)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Badge");

            return claimsIdentity;
        }
    }

    public void Verify() { }
}