using ChristianSchulz.MultitenancyMonolith.Application.Authorization.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Authorization.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authorization;

public interface IMemberRequestHandler
{
    ValueTask<MemberResponse> GetAsync(string uniqueName);

    IAsyncEnumerable<MemberResponse> GetAll();

    ValueTask<MemberResponse> InsertAsync(MemberRequest request);

    ValueTask UpdateAsync(string uniqueName, MemberRequest request);

    ValueTask DeleteAsync(string uniqueName);
}