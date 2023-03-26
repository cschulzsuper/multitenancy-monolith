using ChristianSchulz.MultitenancyMonolith.Application.Admission.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Admission.Responses;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

public interface IAuthenticationRegistrationRequestHandler
{
    Task<AuthenticationRegistrationResponse> GetAsync(long authenticationRegistration);

    IQueryable<AuthenticationRegistrationResponse> GetAll();

    Task<AuthenticationRegistrationResponse> InsertAsync(AuthenticationRegistrationRequest request);

    Task UpdateAsync(long authenticationRegistration, AuthenticationRegistrationRequest request);

    Task DeleteAsync(long authenticationRegistration);
}