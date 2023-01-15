using ChristianSchulz.MultitenancyMonolith.Application.Administration.Requests;
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

    private readonly string[] _allowedClients = { "swagger", "endpoint-tests" };

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

    public ClaimsIdentity SignIn(string group, string member, MemberSignInRequest request)
    {
        var client = request.Client;

        if (!_allowedClients.Contains(request.Client))
        {
            throw new TransportException($"Client '{client}' is not allowed to sign in");
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
                throw new TransportException($"Member '{member}' does not exist in group '{group}'");
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