using System.Collections.Generic;
using System.Threading.Tasks;
using ChristianSchulz.MultitenancyMonolith.Application.Access.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Access.Responses;
using ChristianSchulz.MultitenancyMonolith.Objects.Access;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

internal sealed class AccountMemberRequestHandler : IAccountMemberRequestHandler
{
    private readonly IAccountMemberManager _accountMemberManager;

    public AccountMemberRequestHandler(IAccountMemberManager accountMemberManager)
    {
        _accountMemberManager = accountMemberManager;
    }

    public async Task<AccountMemberResponse> GetAsync(string accountMember)
    {
        var @object = await _accountMemberManager.GetAsync(accountMember);

        var response = new AccountMemberResponse
        {
            UniqueName = @object.UniqueName,
            MailAddress = @object.MailAddress
        };

        return response;
    }

    public async IAsyncEnumerable<AccountMemberResponse> GetAll()
    {
        var objects = _accountMemberManager.GetAsyncEnumerable();

        await foreach (var @object in objects)
        {
            var response = new AccountMemberResponse
            {
                UniqueName = @object.UniqueName,
                MailAddress = @object.MailAddress
            };

            yield return response;
        }
    }

    public async Task<AccountMemberResponse> InsertAsync(AccountMemberRequest request)
    {
        var @object = new AccountMember
        {
            UniqueName = request.UniqueName,
            MailAddress = request.MailAddress
        };

        await _accountMemberManager.InsertAsync(@object);

        var response = new AccountMemberResponse
        {
            UniqueName = @object.UniqueName,
            MailAddress = @object.MailAddress
        };

        return response;
    }

    public async Task UpdateAsync(string accountMember, AccountMemberRequest request)
        => await _accountMemberManager.UpdateAsync(accountMember,
            @object =>
            {
                @object.UniqueName = request.UniqueName;
                @object.MailAddress = request.MailAddress;
            });

    public async Task DeleteAsync(string accountMember)
        => await _accountMemberManager.DeleteAsync(accountMember);
}