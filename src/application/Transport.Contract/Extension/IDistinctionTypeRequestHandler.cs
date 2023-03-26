using ChristianSchulz.MultitenancyMonolith.Application.Extension.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Extension.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Extension;

public interface IDistinctionTypeRequestHandler
{
    Task<DistinctionTypeResponse> GetAsync(string uniqueName);

    IAsyncEnumerable<DistinctionTypeResponse> GetAll();

    Task<DistinctionTypeResponse> InsertAsync(DistinctionTypeRequest request);

    Task UpdateAsync(string uniqueName, DistinctionTypeRequest request);

    Task DeleteAsync(string uniqueName);

}