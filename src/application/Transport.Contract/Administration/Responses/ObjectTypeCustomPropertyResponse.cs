namespace ChristianSchulz.MultitenancyMonolith.Application.Administration.Responses;

public class ObjectTypeCustomPropertyResponse
{
    public required string UniqueName { get; init; }

    public required string DisplayName { get; init; }

    public required string PropertyType { get; init; }

    public required string PropertyName { get; init; }

    public required string ObjectType { get; init; }
}