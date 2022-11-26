using ChristianSchulz.MultitenancyMonolith.Shared.Security.Claims;
using System;
using System.Linq;
using System.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal sealed class MemberSignInRequestHandler : IMemberSignInRequestHandler
{
    private readonly IMemberManager _memberManager;
    private readonly IMemberVerficationManager _memberVerficationManager;
    private readonly ClaimsPrincipal _user;

    private readonly static object _signInLock = new();

    public MemberSignInRequestHandler(
        IMemberManager memberManager,
        IMemberVerficationManager memberVerficationManager,
        ClaimsPrincipal user)
    {
        _memberManager = memberManager;
        _memberVerficationManager = memberVerficationManager;
        _user = user;
    }

    public ClaimsIdentity SignIn(string group, string member)
    {
        lock (_signInLock)
        {
            var identity = _user.GetClaim("Identity");

            var found = _memberManager
                .GetAll(group)
                .Any(x =>
                    x.UniqueName == member &&
                    x.Identity == identity);

            if (!found)
            {
                throw new TransportException($"Member '{member}' does not exist in group '{group}'");
            }

            var verfication = Guid.NewGuid().ToByteArray();

            _memberVerficationManager.Set(group, member, verfication);

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