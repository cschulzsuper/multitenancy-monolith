using System;
using ChristianSchulz.MultitenancyMonolith.Application.Administration.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Administration.Responses;
using ChristianSchulz.MultitenancyMonolith.Objects.Administration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal sealed class ObjectTypeCustomPropertyRequestHandler : IObjectTypeCustomPropertyRequestHandler
{
    private readonly IObjectTypeManager _objectTypeManager;

    private readonly Action<ObjectType, ObjectTypeCustomPropertyRequest> _insertAction =
        (objectType, request) =>
        {
            var objectTypeCustomProperty = new ObjectTypeCustomProperty
            {
                UniqueName = request.UniqueName,
                DisplayName = request.DisplayName,
                PropertyName = request.PropertyName,
                PropertyType = request.PropertyType
            };

            objectType.CustomProperties.Add(objectTypeCustomProperty);
        };

    private readonly Action<ObjectType, string> _deleteAction =
        (objectType, uniqueName) =>
        {
            var objectTypeCustomProperty = objectType.CustomProperties
                .SingleOrDefault(x => x.UniqueName == uniqueName);

            if (objectTypeCustomProperty == null)
            {
                TransportException.ThrowNotFound<ObjectTypeCustomProperty>(uniqueName);
            }

            objectType.CustomProperties.Remove(objectTypeCustomProperty);
        };

    public ObjectTypeCustomPropertyRequestHandler(IObjectTypeManager objectTypeManager)
    {
        _objectTypeManager = objectTypeManager;
    }

    public async ValueTask<ObjectTypeCustomPropertyResponse> GetAsync(string objectType, string uniqueName)
    {
        ObjectTypeCustomPropertyRequestValidation.EnsureUniqueName(uniqueName);

        var objectTypeGet = await _objectTypeManager.GetOrDefaultAsync(objectType);
        if (objectTypeGet == null)
        {
            TransportException.ThrowNotFound<ObjectTypeCustomProperty>(uniqueName);
        }

        var objectTypeCustomProperty = objectTypeGet.CustomProperties.SingleOrDefault(x => x.UniqueName == uniqueName);
        if (objectTypeCustomProperty == null)
        {
            TransportException.ThrowNotFound<ObjectTypeCustomProperty>(uniqueName);
        }

        var response = new ObjectTypeCustomPropertyResponse
        {
            UniqueName = objectTypeCustomProperty.UniqueName,
            DisplayName = objectTypeCustomProperty.DisplayName,
            PropertyName = objectTypeCustomProperty.PropertyName,
            PropertyType = objectTypeCustomProperty.PropertyType,
            ObjectType = objectType
        };

        return response;
    }

    public async IAsyncEnumerable<ObjectTypeCustomPropertyResponse> GetAll(string objectType)
    {
        var objectTypeGet = await _objectTypeManager.GetOrDefaultAsync(objectType);
        if (objectTypeGet == null)
        {
            yield break;
        }

        var objectTypeCustomProperties = objectTypeGet.CustomProperties
            .OrderBy(x => x.UniqueName);

        foreach (var objectTypeCustomProperty in objectTypeCustomProperties)
        {
            var response = new ObjectTypeCustomPropertyResponse
            {
                UniqueName = objectTypeCustomProperty.UniqueName,
                DisplayName = objectTypeCustomProperty.DisplayName,
                PropertyName = objectTypeCustomProperty.PropertyName,
                PropertyType = objectTypeCustomProperty.PropertyType,
                ObjectType = objectType
            };

            yield return response;
        }
    }

    public async ValueTask<ObjectTypeCustomPropertyResponse> InsertAsync(string objectType, ObjectTypeCustomPropertyRequest request)
    {
        var objectTypeExists = await _objectTypeManager.ExistsAsync(objectType);

        if (objectTypeExists)
        {
            await _objectTypeManager.UpdateAsync(objectType,
                objectTypeUpdate => _insertAction.Invoke(objectTypeUpdate, request));
        }
        else
        {
            var objectTypeInsert = new ObjectType
            {
                UniqueName = objectType
            };

            _insertAction.Invoke(objectTypeInsert, request);

            await _objectTypeManager.InsertAsync(objectTypeInsert);
        }

        var response = new ObjectTypeCustomPropertyResponse
        {
            UniqueName = request.UniqueName,
            DisplayName = request.DisplayName,
            PropertyName = request.PropertyName,
            PropertyType = request.PropertyType,
            ObjectType = objectType
        };

        return response;
    }

    public async ValueTask UpdateAsync(string objectType, string uniqueName, ObjectTypeCustomPropertyRequest request)
    {
        ObjectTypeCustomPropertyRequestValidation.EnsureUniqueName(uniqueName);

        await _objectTypeManager.UpdateAsync(objectType,
            objectTypeUpdate =>
            {
                _deleteAction.Invoke(objectTypeUpdate, uniqueName);
                _insertAction.Invoke(objectTypeUpdate, request);
            });
    }

    public async ValueTask DeleteAsync(string objectType, string uniqueName)
    {
        ObjectTypeCustomPropertyRequestValidation.EnsureUniqueName(uniqueName);

        await _objectTypeManager.UpdateAsync(objectType,
            objectTypeUpdate => _deleteAction.Invoke(objectTypeUpdate, uniqueName));
    }
}