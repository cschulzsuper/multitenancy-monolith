namespace ChristianSchulz.MultitenancyMonolith.Application.Extension.Responses;

public sealed class ObjectTypeResponse
{
    public required string UniqueName { get; init; }

    public required string DisplayName { get; init; }

    public required string Area { get; init; }

    public required string Collection { get; init; }

}