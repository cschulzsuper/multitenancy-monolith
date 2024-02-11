using ChristianSchulz.MultitenancyMonolith.Application.Extension.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Extension.Responses;
using ChristianSchulz.MultitenancyMonolith.Objects.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Extension;

internal sealed class DistinctionTypeCustomPropertyRequestHandler : IDistinctionTypeCustomPropertyRequestHandler
{
    private readonly IDistinctionTypeManager _distinctionTypeManager;

    private readonly Action<DistinctionType, DistinctionTypeCustomPropertyRequest> _insertAction =
        (distinctionType, request) =>
        {
            var distinctionTypeCustomProperty = new DistinctionTypeCustomProperty
            {
                UniqueName = request.UniqueName
            };

            distinctionType.CustomProperties.Add(distinctionTypeCustomProperty);
        };

    private readonly Action<DistinctionType, string> _deleteAction =
        (distinctionType, uniqueName) =>
        {
            var distinctionTypeCustomProperty = distinctionType.CustomProperties
                .SingleOrDefault(x => x.UniqueName == uniqueName);

            if (distinctionTypeCustomProperty == null)
            {
                TransportException.ThrowNotFound<DistinctionTypeCustomProperty>(uniqueName);
            }

            distinctionType.CustomProperties.Remove(distinctionTypeCustomProperty);
        };

    public DistinctionTypeCustomPropertyRequestHandler(IDistinctionTypeManager distinctionTypeManager)
    {
        _distinctionTypeManager = distinctionTypeManager;
    }

    public async Task<DistinctionTypeCustomPropertyResponse> GetAsync(string distinctionType, string uniqueName)
    {
        DistinctionTypeCustomPropertyRequestValidation.EnsureUniqueName(uniqueName);

        var distinctionTypeGet = await _distinctionTypeManager.GetAsync(distinctionType);

        var distinctionTypeCustomProperty = distinctionTypeGet.CustomProperties
            .SingleOrDefault(x => x.UniqueName == uniqueName);

        if (distinctionTypeCustomProperty == null)
        {
            TransportException.ThrowNotFound<DistinctionTypeCustomProperty>(uniqueName);
        }

        var response = new DistinctionTypeCustomPropertyResponse
        {
            UniqueName = distinctionTypeCustomProperty.UniqueName,
            DistinctionType = distinctionType
        };

        return response;
    }

    public async IAsyncEnumerable<DistinctionTypeCustomPropertyResponse> GetAll(string distinctionType)
    {
        var distinctionTypeGet = await _distinctionTypeManager.GetAsync(distinctionType);

        var distinctionTypeCustomProperties = distinctionTypeGet.CustomProperties
            .OrderBy(x => x.UniqueName);

        foreach (var distinctionTypeCustomProperty in distinctionTypeCustomProperties)
        {
            var response = new DistinctionTypeCustomPropertyResponse
            {
                UniqueName = distinctionTypeCustomProperty.UniqueName,
                DistinctionType = distinctionType
            };

            yield return response;
        }
    }

    public async Task<DistinctionTypeCustomPropertyResponse> InsertAsync(string distinctionType, DistinctionTypeCustomPropertyRequest request)
    {
        await _distinctionTypeManager.UpdateAsync(distinctionType,
            distinctionTypeUpdate => _insertAction.Invoke(distinctionTypeUpdate, request));

        var response = new DistinctionTypeCustomPropertyResponse
        {
            UniqueName = request.UniqueName,
            DistinctionType = distinctionType
        };

        return response;
    }

    public async Task UpdateAsync(string distinctionType, string uniqueName, DistinctionTypeCustomPropertyRequest request)
    {
        DistinctionTypeCustomPropertyRequestValidation.EnsureUniqueName(uniqueName);

        await _distinctionTypeManager.UpdateAsync(distinctionType,
            distinctionTypeUpdate =>
            {
                _deleteAction.Invoke(distinctionTypeUpdate, uniqueName);
                _insertAction.Invoke(distinctionTypeUpdate, request);
            });
    }

    public async Task DeleteAsync(string distinctionType, string uniqueName)
    {
        DistinctionTypeCustomPropertyRequestValidation.EnsureUniqueName(uniqueName);

        await _distinctionTypeManager.UpdateAsync(distinctionType,
            distinctionTypeUpdate => _deleteAction.Invoke(distinctionTypeUpdate, uniqueName));
    }
}