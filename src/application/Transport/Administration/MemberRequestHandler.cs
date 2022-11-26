using ChristianSchulz.MultitenancyMonolith.Application.Administration.Responses;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Claims;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal sealed class MemberRequestHandler : IMemberRequestHandler
{
    private readonly IMemberManager _memberManager;
    private readonly ClaimsPrincipal _user;

    public MemberRequestHandler(
        IMemberManager memberManager,
        ClaimsPrincipal user)
    {
        _memberManager = memberManager;
        _user = user;
    }



    public MemberResponse Get(string uniqueName)
    {
        var group = _user.GetClaim("Group");

        var member = _memberManager.Get(group, uniqueName);

        var response = new MemberResponse
        {
            UniqueName = member.UniqueName,
            Identity = member.Identity
        };

        return response;
    }

    public IEnumerable<MemberResponse> GetAll()
    {
        var group = _user.GetClaim("Group");

        var members = _memberManager.GetAll(group);

        var response = members.Select(member =>
            new MemberResponse
            {
                UniqueName = member.UniqueName,
                Identity = member.Identity
            });

        return response;
    }
}
