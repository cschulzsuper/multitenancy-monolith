namespace ChristianSchulz.MultitenancyMonolith.Application.Schedule.Requests;

public sealed class PlannedJobRequest
{
    public required string Expression { get; init; }
}