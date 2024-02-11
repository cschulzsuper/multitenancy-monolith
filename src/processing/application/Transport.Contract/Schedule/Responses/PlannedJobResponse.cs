namespace ChristianSchulz.MultitenancyMonolith.Application.Schedule.Responses;

public sealed class PlannedJobResponse
{
    public required string UniqueName { get; init; }
    public required string Expression { get; init; }
}