using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration.Requests;

public class DistinctionTypeCustomPropertyRequest
{
    [UniqueName]
    public required string UniqueName { get; init; }
}