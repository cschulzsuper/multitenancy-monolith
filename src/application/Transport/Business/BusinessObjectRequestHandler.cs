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

    public async Task<BusinessObjectResponse> GetAsync(string uniqueName)
    {
        var businessObject = await _businessObjectManager.GetAsync(uniqueName);

        var response = new BusinessObjectResponse
        {
            UniqueName = businessObject.UniqueName,
            CustomProperties = new Dictionary<string, object>(businessObject.CustomProperties)
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
                UniqueName = businessObject.UniqueName,
                CustomProperties = new Dictionary<string, object>(businessObject.CustomProperties)
            };

            yield return response;
        }
    }

    public async Task<BusinessObjectResponse> InsertAsync(BusinessObjectRequest request)
    {
        var businessObject = new BusinessObject
        {
            UniqueName = request.UniqueName,
            CustomProperties = new Dictionary<string, object>(request.CustomProperties)
        };

        await _businessObjectManager.InsertAsync(businessObject);

        var response = new BusinessObjectResponse
        {
            UniqueName = businessObject.UniqueName,
            CustomProperties = new Dictionary<string, object>(businessObject.CustomProperties)
        };

        return response;
    }

    public async Task UpdateAsync(string uniqueName, BusinessObjectRequest request)
    {
        await _businessObjectManager.UpdateAsync(uniqueName,
            businessObject =>
            {
                businessObject.UniqueName = request.UniqueName;
                businessObject.CustomProperties = new Dictionary<string, object>(request.CustomProperties);
            });
    }

    public async Task DeleteAsync(string uniqueName)
        => await _businessObjectManager.DeleteAsync(uniqueName);
}