using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

internal sealed class AuthenticationIdentityAuthenticationMethodManager : IAuthenticationIdentityAuthenticationMethodManager
{
    private readonly IRepository<AuthenticationIdentityAuthenticationMethod> _repository;

    public AuthenticationIdentityAuthenticationMethodManager(IRepository<AuthenticationIdentityAuthenticationMethod> repository)
    {
        _repository = repository;
    }

    public async Task<bool> ExistsAsync(long authenticationIdentity, string clientName, string authenticationMethod)
    {
        AuthenticationIdentityAuthenticationMethodValidation.EnsureAuthenticationIdentity(authenticationIdentity);
        AuthenticationIdentityAuthenticationMethodValidation.EnsureClientName(clientName);
        AuthenticationIdentityAuthenticationMethodValidation.EnsureAuthenticationMethod(authenticationMethod);

        var exists = await _repository
            .ExistsAsync(@object =>
                @object.AuthenticationIdentity == authenticationIdentity &&
                @object.ClientName == clientName &&
                @object.AuthenticationMethod == authenticationMethod);

        return exists;
    }
}