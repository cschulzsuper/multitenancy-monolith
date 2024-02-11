using ChristianSchulz.MultitenancyMonolith.Application.Business.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Business.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Business;

public interface IBusinessObjectRequestHandler
{
    Task<BusinessObjectResponse> GetAsync(string uniqueName);

    IAsyncEnumerable<BusinessObjectResponse> GetAll();

    Task<BusinessObjectResponse> InsertAsync(BusinessObjectRequest request);

    Task UpdateAsync(string uniqueName, BusinessObjectRequest request);

    Task DeleteAsync(string uniqueName);
}