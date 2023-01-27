namespace ChristianSchulz.MultitenancyMonolith.Application.Administration.Responses;

public class AggregateTypeResponse
{
    public required string UniqueName { get; init; }

    public required string DisplayName { get; init; }

    public required string Area { get; init; }

    public required string Resource { get; init; }

}