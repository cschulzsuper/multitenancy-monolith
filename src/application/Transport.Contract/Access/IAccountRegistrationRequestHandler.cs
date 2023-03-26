using ChristianSchulz.MultitenancyMonolith.Application.Access.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Access.Responses;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

public interface IAccountRegistrationRequestHandler
{
    Task<AccountRegistrationResponse> GetAsync(long accountRegistration);

    IQueryable<AccountRegistrationResponse> GetAll();

    Task<AccountRegistrationResponse> InsertAsync(AccountRegistrationRequest request);

    Task UpdateAsync(long accountRegistration, AccountRegistrationRequest request);

    Task DeleteAsync(long accountRegistration);
}