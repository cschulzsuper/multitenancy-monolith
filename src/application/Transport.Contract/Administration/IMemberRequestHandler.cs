using System.Linq;
using System.Threading.Tasks;
using ChristianSchulz.MultitenancyMonolith.Application.Administration.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Administration.Responses;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public interface IMemberRequestHandler
{
    ValueTask<MemberResponse> GetAsync(string uniqueName);

    IQueryable<MemberResponse> GetAll();

    ValueTask<MemberResponse> InsertAsync(MemberRequest request);
    
    ValueTask DeleteAsync(string uniqueName);
}
