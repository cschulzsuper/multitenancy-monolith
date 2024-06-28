using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;
using System.Collections.Generic;

namespace ChristianSchulz.MultitenancyMonolith.Application.Business.Requests;

public sealed class BusinessObjectRequest
{
    [UniqueName]
    public required string UniqueName { get; init; }

    public IDictionary<string, object> CustomProperties { get; init; } = new Dictionary<string, object>();
}