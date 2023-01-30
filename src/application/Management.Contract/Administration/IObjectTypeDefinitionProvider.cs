using System.Collections.Generic;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public interface IObjectTypeDefinitionProvider
{
    ObjectTypeDefinition Get(string uniqueName);

    IEnumerable<ObjectTypeDefinition> GetEnumerable();
}