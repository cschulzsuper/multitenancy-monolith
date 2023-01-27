using ChristianSchulz.MultitenancyMonolith.Application.Administration.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Administration.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public interface IDistinctionTypeCustomPropertyRequestHandler
{
    ValueTask<DistinctionTypeCustomPropertyResponse> GetAsync(string distinctionType, string uniqueName);

    IAsyncEnumerable<DistinctionTypeCustomPropertyResponse> GetAll(string distinctionType);

    ValueTask<DistinctionTypeCustomPropertyResponse> InsertAsync(string distinctionType, DistinctionTypeCustomPropertyRequest request);

    ValueTask UpdateAsync(string distinctionType, string uniqueName, DistinctionTypeCustomPropertyRequest request);

    ValueTask DeleteAsync(string distinctionType, string uniqueName);

}