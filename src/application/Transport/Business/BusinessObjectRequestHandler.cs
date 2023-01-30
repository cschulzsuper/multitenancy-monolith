using ChristianSchulz.MultitenancyMonolith.Objects.Business;
using ChristianSchulz.MultitenancyMonolith.Application.Business.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Business.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Business;

internal sealed class BusinessObjectRequestHandler : IBusinessObjectRequestHandler
{

    private readonly IBusinessObjectManager _businessObjectManager;

    public BusinessObjectRequestHandler(IBusinessObjectManager businessObjectManager)
    {
        _businessObjectManager = businessObjectManager;
    }

    public async ValueTask<BusinessObjectResponse> GetAsync(string uniqueName)
    {
        var businessObject = await _businessObjectManager.GetAsync(uniqueName);

        var response = new BusinessObjectResponse
        {
            UniqueName = businessObject.UniqueName
        };

        return response;
    }

    public async IAsyncEnumerable<BusinessObjectResponse> GetAll()
    {
        var businessObjects = _businessObjectManager.GetAsyncEnumerable();

        await foreach (var businessObject in businessObjects)
        {
            var response = new BusinessObjectResponse
            {
                UniqueName = businessObject.UniqueName
            };

            yield return response;
        }
    }

    public async ValueTask<BusinessObjectResponse> InsertAsync(BusinessObjectRequest request)
    {
        var businessObject = new BusinessObject
        {
            UniqueName = request.UniqueName
        };

        await _businessObjectManager.InsertAsync(businessObject);

        var response = new BusinessObjectResponse
        {
            UniqueName = businessObject.UniqueName
        };

        return response;
    }

    public async ValueTask UpdateAsync(string uniqueName, BusinessObjectRequest request)
    {
        await _businessObjectManager.UpdateAsync(uniqueName,
            businessObject =>
            {
                businessObject.UniqueName = request.UniqueName;
            });
    }

    public async ValueTask DeleteAsync(string uniqueName)
        => await _businessObjectManager.DeleteAsync(uniqueName);
}