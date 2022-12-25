using ChristianSchulz.MultitenancyMonolith.Application.Authentication.Responses;
using System.Linq;
using System.Threading.Tasks;
using ChristianSchulz.MultitenancyMonolith.Aggregates.Authentication;
using ChristianSchulz.MultitenancyMonolith.Application.Authentication.Requests;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

internal sealed class IdentityRequestHandler : IIdentityRequestHandler
{
    private readonly IIdentityManager _identityManager;

    public IdentityRequestHandler(IIdentityManager identityManager)
    {
        _identityManager = identityManager;
    }

    public async ValueTask<IdentityResponse> GetAsync(string uniqueName)
    {
        var identity = await _identityManager.GetAsync(uniqueName);

        var response = new IdentityResponse
        {
            UniqueName = identity.UniqueName,
            MailAddress = identity.MailAddress
        };

        return response;
    }

    public IQueryable<IdentityResponse> GetAll()
    {
        var identities = _identityManager.GetAll();

        var response = identities.Select(member =>
            new IdentityResponse
            {
                UniqueName = member.UniqueName,
                MailAddress = member.MailAddress
            });

        return response;
    }

    public async ValueTask<IdentityResponse> InsertAsync(IdentityRequest request)
    {
        var member = new Identity
        {
            UniqueName = request.UniqueName,
            MailAddress = request.MailAddress,
            Secret = request.Secret
        };

        await _identityManager.InsertAsync(member);

        var response = new IdentityResponse
        {
            UniqueName = member.UniqueName,
            MailAddress = member.MailAddress
        };

        return response;
    }

    public async ValueTask UpdateAsync(string uniqueName, IdentityRequest request)
    => await _identityManager.UpdateAsync(uniqueName, 
        member => 
        {
            member.UniqueName = request.UniqueName;
            member.Secret = request.Secret;
            member.MailAddress = request.MailAddress;
        });

    public async ValueTask DeleteAsync(string uniqueName)
        => await _identityManager.DeleteAsync(uniqueName);
}
