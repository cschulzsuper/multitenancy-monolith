using ChristianSchulz.MultitenancyMonolith.Application.Access.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Access.Responses;
using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

internal sealed class AccountGroupRequestHandler : IAccountGroupRequestHandler
{
    private readonly IAccountGroupManager _accountGroupManager;

    public AccountGroupRequestHandler(IAccountGroupManager accountGroupManager)
    {
        _accountGroupManager = accountGroupManager;
    }

    public async Task HeadAsync(string accountGroup)
    {
        var exists = await _accountGroupManager.ExistsAsync(accountGroup);

        if (!exists)
        {
            TransportException.ThrowNotFound<AccountGroup>(accountGroup);
        }
    }

    public async Task<AccountGroupResponse> GetAsync(string accountGroup)
    {
        var @object = await _accountGroupManager.GetAsync(accountGroup);

        var response = new AccountGroupResponse
        {
            UniqueName = @object.UniqueName
        };

        return response;
    }

    public IQueryable<AccountGroupResponse> GetAll()
    {
        var objects = _accountGroupManager.GetAll();

        var response = objects.Select(@object =>
            new AccountGroupResponse
            {
                UniqueName = @object.UniqueName
            });

        return response;
    }

    public async Task<AccountGroupResponse> InsertAsync(AccountGroupRequest request)
    {
        var @object = new AccountGroup
        {
            UniqueName = request.UniqueName
        };

        await _accountGroupManager.InsertAsync(@object);

        var response = new AccountGroupResponse
        {
            UniqueName = @object.UniqueName
        };

        return response;
    }

    public async Task UpdateAsync(string accountGroup, AccountGroupRequest request)
    => await _accountGroupManager.UpdateAsync(accountGroup,
        @object =>
        {
            @object.UniqueName = request.UniqueName;
        });

    public async Task DeleteAsync(string accountGroup)
        => await _accountGroupManager.DeleteAsync(accountGroup);
}