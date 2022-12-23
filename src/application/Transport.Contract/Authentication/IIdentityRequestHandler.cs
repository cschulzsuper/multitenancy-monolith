using System.Linq;
using System.Threading.Tasks;
using ChristianSchulz.MultitenancyMonolith.Application.Authentication.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Authentication.Responses;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

public interface IIdentityRequestHandler
{
    ValueTask<IdentityResponse> GetAsync(string uniqueName);

    IQueryable<IdentityResponse> GetAll();

    ValueTask<IdentityResponse> InsertAsync(IdentityRequest request);

    ValueTask DeleteAsync(string uniqueName);
}