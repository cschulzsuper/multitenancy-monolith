namespace ChristianSchulz.MultitenancyMonolith.Application.Extension.Responses;

public sealed class ObjectTypeCustomPropertyResponse
{
    public required string UniqueName { get; init; }

    public required string DisplayName { get; init; }

    public required string PropertyType { get; init; }

    public required string PropertyName { get; init; }

    public required string ObjectType { get; init; }
}