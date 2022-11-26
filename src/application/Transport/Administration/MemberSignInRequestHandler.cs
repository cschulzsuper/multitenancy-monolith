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

    public ClaimsIdentity SignIn(string group, string uniqueName)
    {
        lock (_signInLock)
        {
            var userIdentity = _user.GetClaim("Identity");

            var member = _memberManager.GetAll(group)
                .Single(x =>
                    x.UniqueName == uniqueName &&
                    x.Identity == userIdentity);

            var memberVerfication = Guid
                .NewGuid()
                .ToByteArray();

            _memberVerficationManager.Set($"{group}.{member.UniqueName}", memberVerfication);

            var memberVerificationString = Convert.ToBase64String(memberVerfication);

            var claims = new Claim[]
            {
                new Claim("Identity", userIdentity),
                new Claim("Group", group),
                new Claim("Member", uniqueName),
                new Claim("Verification", memberVerificationString, ClaimValueTypes.Base64Binary)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Badge");

            return claimsIdentity;
        }
    }

    public void Verify() { }
}