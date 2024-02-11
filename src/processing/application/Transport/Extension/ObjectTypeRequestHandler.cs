using ChristianSchulz.MultitenancyMonolith.Application.Extension.Responses;
using System.Collections.Generic;

namespace ChristianSchulz.MultitenancyMonolith.Application.Extension;

internal sealed class ObjectTypeRequestHandler : IObjectTypeRequestHandler
{
    private readonly IObjectTypeDefinitionProvider _objectTypeDefinitionProvider;

    public ObjectTypeRequestHandler(IObjectTypeDefinitionProvider objectTypeDefinitionProvider)
    {
        _objectTypeDefinitionProvider = objectTypeDefinitionProvider;
    }

    public ObjectTypeResponse Get(string uniqueName)
    {
        var objectType = _objectTypeDefinitionProvider.Get(uniqueName);

        var response = new ObjectTypeResponse
        {
            UniqueName = objectType.UniqueName,
            DisplayName = objectType.DisplayName,
            Area = objectType.Area,
            Collection = objectType.Collection
        };

        return response;
    }

    public IEnumerable<ObjectTypeResponse> GetAll()
    {
        var objectTypes = _objectTypeDefinitionProvider.GetEnumerable();

        foreach (var objectType in objectTypes)
        {
            var response = new ObjectTypeResponse
            {
                UniqueName = objectType.UniqueName,
                DisplayName = objectType.DisplayName,
                Area = objectType.Area,
                Collection = objectType.Collection
            };

            yield return response;
        }
    }
}