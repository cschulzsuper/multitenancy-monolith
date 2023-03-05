using ChristianSchulz.MultitenancyMonolith.Application.Access.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Access.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

public interface IAccountMemberRequestHandler
{
    Task<AccountMemberResponse> GetAsync(string accountMember);

    IAsyncEnumerable<AccountMemberResponse> GetAll();

    Task<AccountMemberResponse> InsertAsync(AccountMemberRequest request);

    Task UpdateAsync(string accountMember, AccountMemberRequest request);

    Task DeleteAsync(string accountMember);
}