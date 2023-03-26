using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Extension.Requests;

public sealed class DistinctionTypeCustomPropertyRequest
{
    [UniqueName]
    public required string UniqueName { get; init; }
}