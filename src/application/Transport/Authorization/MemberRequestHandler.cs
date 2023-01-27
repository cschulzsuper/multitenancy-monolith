using ChristianSchulz.MultitenancyMonolith.Aggregates.Administration;
using ChristianSchulz.MultitenancyMonolith.Application.Authorization.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Authorization.Responses;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authorization;

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

    public async IAsyncEnumerable<MemberResponse> GetAll()
    {
        var members = _memberManager.GetAsyncEnumerable();

        await foreach(var member in members)
        {
            var response = new MemberResponse
            {
                UniqueName = member.UniqueName
            };

            yield return response;
        }
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