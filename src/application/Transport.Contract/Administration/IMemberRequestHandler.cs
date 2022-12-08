using System.Linq;
using ChristianSchulz.MultitenancyMonolith.Application.Administration.Responses;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public interface IMemberRequestHandler
{
    MemberResponse Get(string uniqueName);

    IQueryable<MemberResponse> GetAll();
}
