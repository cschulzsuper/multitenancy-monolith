using ChristianSchulz.MultitenancyMonolith.Application.Administration.Responses;
using System.Collections.Generic;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal sealed class AggregateTypeRequestHandler : IAggregateTypeRequestHandler
{
    public readonly IDictionary<string,AggregateTypeResponse> _aggregateTypes = new Dictionary<string, AggregateTypeResponse>
    {
        ["business-object"] = new()
        {
            UniqueName = "business-object",
            DisplayName = "Business Object",
            Area = "business",
            Resource = "business-objects"
        }
    };

    public AggregateTypeResponse Get(string uniqueName)
        => _aggregateTypes[uniqueName];

    public IEnumerable<AggregateTypeResponse> GetAll()
        => _aggregateTypes.Values;

}