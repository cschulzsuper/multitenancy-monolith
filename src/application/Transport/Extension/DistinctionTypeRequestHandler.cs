using ChristianSchulz.MultitenancyMonolith.Application.Extension.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Extension.Responses;
using ChristianSchulz.MultitenancyMonolith.Objects.Extension;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Extension;

internal sealed class DistinctionTypeRequestHandler : IDistinctionTypeRequestHandler
{

    private readonly IDistinctionTypeManager _distinctionTypeManager;

    public DistinctionTypeRequestHandler(IDistinctionTypeManager distinctionTypeManager)
    {
        _distinctionTypeManager = distinctionTypeManager;
    }

    public async Task<DistinctionTypeResponse> GetAsync(string uniqueName)
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

    public async Task<DistinctionTypeResponse> InsertAsync(DistinctionTypeRequest request)
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

    public async Task UpdateAsync(string uniqueName, DistinctionTypeRequest request)
    {
        await _distinctionTypeManager.UpdateAsync(uniqueName,
            distinctionType =>
            {
                distinctionType.UniqueName = request.UniqueName;
                distinctionType.DisplayName = request.DisplayName;
                distinctionType.ObjectType = request.ObjectType;
            });
    }

    public async Task DeleteAsync(string uniqueName)
        => await _distinctionTypeManager.DeleteAsync(uniqueName);
}