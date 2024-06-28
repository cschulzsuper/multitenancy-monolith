using ChristianSchulz.MultitenancyMonolith.Objects.Business;
using ChristianSchulz.MultitenancyMonolith.Shared.Metadata;
using System.Collections.Generic;

namespace ChristianSchulz.MultitenancyMonolith.Application.Extension;

internal sealed class ObjectTypeDefinitionProvider : IObjectTypeDefinitionProvider
{
    public static readonly IDictionary<string, ObjectTypeDefinition> _objectTypes;

    static ObjectTypeDefinitionProvider()
    {
        var businessObjectType = typeof(BusinessObject);
        var businessObjectUniqueName = ObjectAnnotations.UniqueName(businessObjectType);

        _objectTypes = new Dictionary<string, ObjectTypeDefinition>
        {
            [businessObjectUniqueName] = new()
            {
                UniqueName = businessObjectUniqueName,
                DisplayName = ObjectAnnotations.DisplayName(businessObjectType),
                Area = ObjectAnnotations.Area(businessObjectType),
                Collection = ObjectAnnotations.Collection(businessObjectType)
            }
        };
    }

    public ObjectTypeDefinition Get(string uniqueName)
    {
        ObjectTypeValidation.EnsureUniqueName(uniqueName);

        return _objectTypes[uniqueName];
    }

    public IEnumerable<ObjectTypeDefinition> GetEnumerable()
        => _objectTypes.Values;
}