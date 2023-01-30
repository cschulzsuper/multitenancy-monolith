using ChristianSchulz.MultitenancyMonolith.Application.Administration.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Administration.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public interface IObjectTypeCustomPropertyRequestHandler
{
    ValueTask<ObjectTypeCustomPropertyResponse> GetAsync(string objectType, string uniqueName);

    IAsyncEnumerable<ObjectTypeCustomPropertyResponse> GetAll(string objectType);

    ValueTask<ObjectTypeCustomPropertyResponse> InsertAsync(string objectType, ObjectTypeCustomPropertyRequest request);

    ValueTask UpdateAsync(string objectType, string uniqueName, ObjectTypeCustomPropertyRequest request);

    ValueTask DeleteAsync(string objectType, string uniqueName);

}