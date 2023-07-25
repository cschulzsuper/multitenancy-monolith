namespace ChristianSchulz.MultitenancyMonolith.Application.Schedule.Requests;

public class PlannedJobRequest
{
    public required string Expression { get; init; }
}