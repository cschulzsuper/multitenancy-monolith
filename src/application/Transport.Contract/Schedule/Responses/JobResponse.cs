namespace ChristianSchulz.MultitenancyMonolith.Application.Schedule.Responses;

public class JobResponse
{
    public required string UniqueName { get; init; }
    public required string Expression { get; init; }
    public required long Timestamp { get; init; }
}