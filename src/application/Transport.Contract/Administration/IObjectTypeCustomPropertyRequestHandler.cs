using ChristianSchulz.MultitenancyMonolith.Application.Administration.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Administration.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public interface IObjectTypeCustomPropertyRequestHandler
{
    Task<ObjectTypeCustomPropertyResponse> GetAsync(string objectType, string uniqueName);

    IAsyncEnumerable<ObjectTypeCustomPropertyResponse> GetAll(string objectType);

    Task<ObjectTypeCustomPropertyResponse> InsertAsync(string objectType, ObjectTypeCustomPropertyRequest request);

    Task UpdateAsync(string objectType, string uniqueName, ObjectTypeCustomPropertyRequest request);

    Task DeleteAsync(string objectType, string uniqueName);

}