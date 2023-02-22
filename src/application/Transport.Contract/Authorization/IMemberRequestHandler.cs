using ChristianSchulz.MultitenancyMonolith.Application.Authorization.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Authorization.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authorization;

public interface IMemberRequestHandler
{
    Task<MemberResponse> GetAsync(string uniqueName);

    IAsyncEnumerable<MemberResponse> GetAll();

    Task<MemberResponse> InsertAsync(MemberRequest request);

    Task UpdateAsync(string uniqueName, MemberRequest request);

    Task DeleteAsync(string uniqueName);
}