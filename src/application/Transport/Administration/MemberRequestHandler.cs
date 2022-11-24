using ChristianSchulz.MultitenancyMonolith.Application.Administration.Responses;
using ChristianSchulz.MultitenancyMonolith.Shared.Security;
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

    private string Group => _user.GetGroupOrDefault()
        ?? throw new TransportException("Could not find 'Group' claim");

    public MemberResponse Get(string uniqueName)
    {
        var member = _memberManager.Get(Group, uniqueName);

        var response = new MemberResponse
        {
            UniqueName = member.UniqueName,
            Identity = member.Identity
        };

        return response;
    }

    public IEnumerable<MemberResponse> GetAll()
    {
        var members = _memberManager.GetAll(Group);

        var response = members.Select(member =>
            new MemberResponse
            {
                UniqueName = member.UniqueName,
                Identity = member.Identity
            });

        return response;
    }
}
