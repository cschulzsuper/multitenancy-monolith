using ChristianSchulz.MultitenancyMonolith.Application.Administration.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Administration.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal sealed class DistinctionTypeCustomPropertyRequestHandler : IDistinctionTypeCustomPropertyRequestHandler
{
    public ValueTask<DistinctionTypeCustomPropertyResponse> GetAsync(string distinctionType, string uniqueName)
    {
        throw new System.NotImplementedException();
    }

    public IAsyncEnumerable<DistinctionTypeCustomPropertyResponse> GetAll(string distinctionType)
    {
        throw new System.NotImplementedException();
    }

    public ValueTask<DistinctionTypeCustomPropertyResponse> InsertAsync(string distinctionType, DistinctionTypeCustomPropertyRequest request)
    {
        throw new System.NotImplementedException();
    }

    public ValueTask UpdateAsync(string distinctionType, string uniqueName, DistinctionTypeCustomPropertyRequest request)
    {
        throw new System.NotImplementedException();
    }

    public ValueTask DeleteAsync(string distinctionType, string uniqueName)
    {
        throw new System.NotImplementedException();
    }
}