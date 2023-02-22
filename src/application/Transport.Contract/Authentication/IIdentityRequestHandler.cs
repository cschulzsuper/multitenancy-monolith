using ChristianSchulz.MultitenancyMonolith.Application.Authentication.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Authentication.Responses;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

public interface IIdentityRequestHandler
{
    Task<IdentityResponse> GetAsync(string uniqueName);

    IQueryable<IdentityResponse> GetAll();

    Task<IdentityResponse> InsertAsync(IdentityRequest request);

    Task UpdateAsync(string uniqueName, IdentityRequest request);

    Task DeleteAsync(string uniqueName);
}