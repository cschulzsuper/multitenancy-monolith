namespace ChristianSchulz.MultitenancyMonolith.Application.Extension.Responses;

public sealed class DistinctionTypeCustomPropertyResponse
{
    public required string UniqueName { get; init; }
    public required string DistinctionType { get; init; }
}