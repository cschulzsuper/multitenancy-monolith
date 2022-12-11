using ChristianSchulz.MultitenancyMonolith.Aggregates.Administration;
using ChristianSchulz.MultitenancyMonolith.Application.Administration.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Administration.Responses;
using System.Data;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal sealed class MemberRequestHandler : IMemberRequestHandler
{
    private readonly IMemberManager _memberManager;

    public MemberRequestHandler(IMemberManager memberManager)
    {
        _memberManager = memberManager;
    }

    public MemberResponse Get(string uniqueName)
    {
        var member = _memberManager.Get(uniqueName);

        var response = new MemberResponse
        {
            UniqueName = member.UniqueName,
        };

        return response;
    }

    public IQueryable<MemberResponse> GetAll()
    {
        var members = _memberManager.GetAll();

        var response = members.Select(member =>
            new MemberResponse
            {
                UniqueName = member.UniqueName,
            });

        return response;
    }

    public MemberResponse Insert(MemberRequest request)
    {
        var member = new Member
        {
            UniqueName = request.UniqueName
        };

        _memberManager.Insert(member);

        var response = new MemberResponse
        {
            UniqueName = member.UniqueName,
        };

        return response;
    }
}
