namespace ChristianSchulz.MultitenancyMonolith.Application.Administration.Responses;

public sealed class DistinctionTypeResponse
{
    public required string UniqueName { get; init; }
    public required string DisplayName { get; init; }
    public required string ObjectType { get; init; }
}