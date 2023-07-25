namespace ChristianSchulz.MultitenancyMonolith.Application.Schedule.Responses;

public class PlannedJobResponse
{
    public required string UniqueName { get; init; }
    public required string Expression { get; init; }
}