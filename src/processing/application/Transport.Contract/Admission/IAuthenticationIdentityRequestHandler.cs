using ChristianSchulz.MultitenancyMonolith.Application.Admission.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Admission.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

public interface IAuthenticationIdentityRequestHandler
{
    Task HeadAsync(string authenticationIdentity);

    Task<AuthenticationIdentityResponse> GetAsync(string authenticationIdentity);

    IAsyncEnumerable<AuthenticationIdentityResponse> GetAll(string? query, int? skip, int? take);

    Task<AuthenticationIdentityResponse> InsertAsync(AuthenticationIdentityRequest request);

    Task UpdateAsync(string authenticationIdentity, AuthenticationIdentityRequest request);

    Task DeleteAsync(string authenticationIdentity);
}