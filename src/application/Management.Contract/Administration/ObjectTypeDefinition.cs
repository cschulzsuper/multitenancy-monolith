namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public sealed class ObjectTypeDefinition
{
    public required string UniqueName { get; init; }

    public required string DisplayName { get; init; }

    public required string Area { get; init; }

    public required string Collection { get; init; }
}