using System.Linq;
using ChristianSchulz.MultitenancyMonolith.Application.Authentication.Responses;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

public interface IIdentityRequestHandler
{
    IdentityResponse Get(string uniqueName);

    IQueryable<IdentityResponse> GetAll();
}