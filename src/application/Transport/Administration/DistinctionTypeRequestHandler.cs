using ChristianSchulz.MultitenancyMonolith.Application.Administration.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Administration.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal sealed class DistinctionTypeRequestHandler : IDistinctionTypeRequestHandler
{
    public ValueTask<DistinctionTypeResponse> GetAsync(string uniqueName)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<DistinctionTypeResponse> GetAll()
    {
        throw new NotImplementedException();
    }

    public ValueTask<DistinctionTypeResponse> InsertAsync(DistinctionTypeRequest request)
    {
        throw new NotImplementedException();
    }

    public ValueTask UpdateAsync(string uniqueName, DistinctionTypeRequest request)
    {
        throw new NotImplementedException();
    }

    public ValueTask DeleteAsync(string uniqueName)
    {
        throw new NotImplementedException();
    }
}