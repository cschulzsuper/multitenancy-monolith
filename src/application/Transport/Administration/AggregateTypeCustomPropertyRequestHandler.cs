using ChristianSchulz.MultitenancyMonolith.Application.Administration;
using ChristianSchulz.MultitenancyMonolith.Application.Administration.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Administration.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal sealed class AggregateTypeCustomPropertyRequestHandler : IAggregateTypeCustomPropertyRequestHandler
{
    public ValueTask<AggregateTypeCustomPropertyResponse> GetAsync(string aggregateType, string uniqueName)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<AggregateTypeCustomPropertyResponse> GetAll(string aggregateType)
    {
        throw new NotImplementedException();
    }

    public ValueTask<AggregateTypeCustomPropertyResponse> InsertAsync(string aggregateType, AggregateTypeCustomPropertyRequest request)
    {
        throw new NotImplementedException();
    }

    public ValueTask UpdateAsync(string aggregateType, string uniqueName, AggregateTypeCustomPropertyRequest request)
    {
        throw new NotImplementedException();
    }

    public ValueTask DeleteAsync(string aggregateType, string uniqueName)
    {
        throw new NotImplementedException();
    }
}