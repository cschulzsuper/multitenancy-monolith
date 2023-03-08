using ChristianSchulz.MultitenancyMonolith.Application.Access;
using ChristianSchulz.MultitenancyMonolith.Application.Admission.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Admission.Responses;
using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

internal sealed class AuthenticationIdentityRequestHandler : IAuthenticationIdentityRequestHandler
{
    private readonly IAuthenticationIdentityManager _authenticationIdentityManager;

    public AuthenticationIdentityRequestHandler(IAuthenticationIdentityManager authenticationIdentityManager)
    {
        _authenticationIdentityManager = authenticationIdentityManager;
    }


    public async Task HeadAsync(string authenticationIdentity)
    {
        var exists = await _authenticationIdentityManager.ExistsAsync(authenticationIdentity);

        if (!exists)
        {
            TransportException.ThrowNotFound<AuthenticationIdentity>(authenticationIdentity);
        }
    }

    public async Task<AuthenticationIdentityResponse> GetAsync(string authenticationIdentity)
    {
        var @object = await _authenticationIdentityManager.GetAsync(authenticationIdentity);

        var response = new AuthenticationIdentityResponse
        {
            UniqueName = @object.UniqueName,
            MailAddress = @object.MailAddress
        };

        return response;
    }

    public IQueryable<AuthenticationIdentityResponse> GetAll()
    {
        var objects = _authenticationIdentityManager.GetQueryable();

        var response = objects.Select(@object =>
            new AuthenticationIdentityResponse
            {
                UniqueName = @object.UniqueName,
                MailAddress = @object.MailAddress
            });

        return response;
    }

    public async Task<AuthenticationIdentityResponse> InsertAsync(AuthenticationIdentityRequest request)
    {
        var @object = new AuthenticationIdentity
        {
            UniqueName = request.UniqueName,
            MailAddress = request.MailAddress,
            Secret = request.Secret
        };

        await _authenticationIdentityManager.InsertAsync(@object);

        var response = new AuthenticationIdentityResponse
        {
            UniqueName = @object.UniqueName,
            MailAddress = @object.MailAddress
        };

        return response;
    }

    public async Task UpdateAsync(string authenticationIdentity, AuthenticationIdentityRequest request)
    => await _authenticationIdentityManager.UpdateAsync(authenticationIdentity,
        @object =>
        {
            @object.UniqueName = request.UniqueName;
            @object.Secret = request.Secret;
            @object.MailAddress = request.MailAddress;
        });

    public async Task DeleteAsync(string authenticationIdentity)
        => await _authenticationIdentityManager.DeleteAsync(authenticationIdentity);
}