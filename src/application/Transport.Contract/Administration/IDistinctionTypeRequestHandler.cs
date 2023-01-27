using ChristianSchulz.MultitenancyMonolith.Application.Administration.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Administration.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public interface IDistinctionTypeRequestHandler
{
    ValueTask<DistinctionTypeResponse> GetAsync(string uniqueName);

    IAsyncEnumerable<DistinctionTypeResponse> GetAll();

    ValueTask<DistinctionTypeResponse> InsertAsync(DistinctionTypeRequest request);

    ValueTask UpdateAsync(string uniqueName, DistinctionTypeRequest request);

    ValueTask DeleteAsync(string uniqueName);

}