using System;
using System.Linq;
using System.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public class AuthorizationRequestHandler : IAuthorizationRequestHandler
{
    private readonly IMemberManager _memberManager;

    private readonly static object _signInLock = new();

    public AuthorizationRequestHandler(
        IMemberManager memberManager)
    {
        _memberManager = memberManager;
    }

    public ClaimsIdentity TakeUp(ClaimsPrincipal user, string group, string uniqueName)
    {
        lock (_signInLock)
        {
            var userIdentity = user.Claims.Single(x => x.Type == "Identity").Value;

            var member = _memberManager.GetAll(group)
                .Single(x => 
                    x.UniqueName == uniqueName &&
                    x.Identity == userIdentity);

            member.Verification = Guid
                .NewGuid()
                .ToByteArray();

            var memberVerificationString = Convert.ToBase64String(member.Verification);

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