using ChristianSchulz.MultitenancyMonolith.Application.Business.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Business.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Business;

public interface IBusinessObjectRequestHandler
{
    ValueTask<BusinessObjectResponse> GetAsync(string uniqueName);

    IAsyncEnumerable<BusinessObjectResponse> GetAll();

    ValueTask<BusinessObjectResponse> InsertAsync(BusinessObjectRequest request);

    ValueTask UpdateAsync(string uniqueName, BusinessObjectRequest request);

    ValueTask DeleteAsync(string uniqueName);
}