using ChristianSchulz.MultitenancyMonolith.Aggregates.Administration;
using ChristianSchulz.MultitenancyMonolith.Application.Administration.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Administration.Responses;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal sealed class MemberRequestHandler : IMemberRequestHandler
{
    private readonly IMemberManager _memberManager;

    public MemberRequestHandler(IMemberManager memberManager)
    {
        _memberManager = memberManager;
    }

    public async ValueTask<MemberResponse> GetAsync(string uniqueName)
    {
        var member = await _memberManager.GetAsync(uniqueName);

        var response = new MemberResponse
        {
            UniqueName = member.UniqueName,
        };

        return response;
    }

    public IQueryable<MemberResponse> GetAll()
    {
        var members = _memberManager.GetQueryable();

        var response = members.Select(member =>
            new MemberResponse
            {
                UniqueName = member.UniqueName,
            });

        return response;
    }

    public async ValueTask<MemberResponse> InsertAsync(MemberRequest request)
    {
        var member = new Member
        {
            UniqueName = request.UniqueName
        };

        await _memberManager.InsertAsync(member);

        var response = new MemberResponse
        {
            UniqueName = member.UniqueName,
        };

        return response;
    }

    public async ValueTask UpdateAsync(string uniqueName, MemberRequest request)
        => await _memberManager.UpdateAsync(uniqueName,
            member =>
            {
                member.UniqueName = request.UniqueName;
            });

    public async ValueTask DeleteAsync(string uniqueName)
        => await _memberManager.DeleteAsync(uniqueName);
}
