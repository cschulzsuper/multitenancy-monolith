using System.Collections.Generic;

namespace ChristianSchulz.MultitenancyMonolith.Application.Extension;

public interface IObjectTypeDefinitionProvider
{
    ObjectTypeDefinition Get(string uniqueName);

    IEnumerable<ObjectTypeDefinition> GetEnumerable();
}