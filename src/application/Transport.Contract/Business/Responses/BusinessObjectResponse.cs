using System.Collections.Generic;

namespace ChristianSchulz.MultitenancyMonolith.Application.Business.Responses;

public class BusinessObjectResponse
{
    public required string UniqueName { get; init; }

    public required IDictionary<string, object> CustomProperties { get; init; } = new Dictionary<string, object>();
}