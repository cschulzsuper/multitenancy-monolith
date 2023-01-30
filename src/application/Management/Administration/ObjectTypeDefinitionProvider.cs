using System.Collections.Generic;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal sealed class ObjectTypeDefinitionProvider : IObjectTypeDefinitionProvider
{
    public readonly IDictionary<string, ObjectTypeDefinition> _objectTypes = new Dictionary<string, ObjectTypeDefinition>
    {
        ["business-object"] = new()
        {
            UniqueName = "business-object",
            DisplayName = "Business Object",
            Area = "business",
            Collection = "business-objects"
        }
    };

    public ObjectTypeDefinition Get(string uniqueName)
    {
        ObjectTypeValidation.EnsureUniqueName(uniqueName);

        return _objectTypes[uniqueName];
    }

    public IEnumerable<ObjectTypeDefinition> GetEnumerable()
        => _objectTypes.Values;
}