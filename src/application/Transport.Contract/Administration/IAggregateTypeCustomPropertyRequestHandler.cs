using ChristianSchulz.MultitenancyMonolith.Application.Administration.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Administration.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public interface IAggregateTypeCustomPropertyRequestHandler
{
    ValueTask<AggregateTypeCustomPropertyResponse> GetAsync(string aggregateType, string uniqueName);

    IAsyncEnumerable<AggregateTypeCustomPropertyResponse> GetAll(string aggregateType);

    ValueTask<AggregateTypeCustomPropertyResponse> InsertAsync(string aggregateType, AggregateTypeCustomPropertyRequest request);

    ValueTask UpdateAsync(string aggregateType, string uniqueName, AggregateTypeCustomPropertyRequest request);

    ValueTask DeleteAsync(string aggregateType, string uniqueName);

}