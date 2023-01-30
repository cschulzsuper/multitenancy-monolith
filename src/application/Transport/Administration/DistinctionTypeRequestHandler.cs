using ChristianSchulz.MultitenancyMonolith.Application.Administration.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Administration.Responses;
using ChristianSchulz.MultitenancyMonolith.Objects.Administration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal sealed class DistinctionTypeRequestHandler : IDistinctionTypeRequestHandler
{
    
    private readonly IDistinctionTypeManager _distinctionTypeManager;

    public DistinctionTypeRequestHandler(IDistinctionTypeManager distinctionTypeManager)
    {
        _distinctionTypeManager = distinctionTypeManager;
    }

    public async ValueTask<DistinctionTypeResponse> GetAsync(string uniqueName)
    {
        var distinctionType = await _distinctionTypeManager.GetAsync(uniqueName);

        var response = new DistinctionTypeResponse
        {
            UniqueName = distinctionType.UniqueName,
            DisplayName = distinctionType.DisplayName,
            ObjectType = distinctionType.ObjectType
        };

        return response;
    }

    public async IAsyncEnumerable<DistinctionTypeResponse> GetAll()
    {
        var distinctionTypes = _distinctionTypeManager.GetAsyncEnumerable();

        await foreach (var distinctionType in distinctionTypes)
        {
            var response = new DistinctionTypeResponse
            {
                UniqueName = distinctionType.UniqueName,
                DisplayName = distinctionType.DisplayName,
                ObjectType = distinctionType.ObjectType
            };

            yield return response;
        }
    }

    public async ValueTask<DistinctionTypeResponse> InsertAsync(DistinctionTypeRequest request)
    {
        var distinctionType = new DistinctionType
        {
            UniqueName = request.UniqueName,
            DisplayName = request.DisplayName,
            ObjectType = request.ObjectType
        };

        await _distinctionTypeManager.InsertAsync(distinctionType);

        var response = new DistinctionTypeResponse
        {
            UniqueName = distinctionType.UniqueName,
            DisplayName = distinctionType.DisplayName,
            ObjectType = distinctionType.ObjectType
        };

        return response;
    }

    public async ValueTask UpdateAsync(string uniqueName, DistinctionTypeRequest request)
    {
        await _distinctionTypeManager.UpdateAsync(uniqueName,
            distinctionType =>
            {
                distinctionType.UniqueName = request.UniqueName;
                distinctionType.DisplayName = request.DisplayName;
                distinctionType.ObjectType = request.ObjectType;
            });
    }

    public async ValueTask DeleteAsync(string uniqueName)
        => await _distinctionTypeManager.DeleteAsync(uniqueName);
}