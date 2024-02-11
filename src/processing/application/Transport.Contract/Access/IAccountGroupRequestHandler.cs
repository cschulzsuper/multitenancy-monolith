using ChristianSchulz.MultitenancyMonolith.Application.Access.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Access.Responses;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

public interface IAccountGroupRequestHandler
{
    Task HeadAsync(string accountGroup);

    Task<AccountGroupResponse> GetAsync(string accountGroup);

    IQueryable<AccountGroupResponse> GetAll();

    Task<AccountGroupResponse> InsertAsync(AccountGroupRequest request);

    Task UpdateAsync(string accountGroup, AccountGroupRequest request);

    Task DeleteAsync(string accountGroup);
}