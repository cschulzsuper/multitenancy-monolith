using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration.Requests;

public sealed class DistinctionTypeRequest
{
    [UniqueName]
    public required string UniqueName { get; init; }

    [DisplayName]
    public required string DisplayName { get; init; }

    [UniqueName]
    public required string ObjectType { get; init; }
}