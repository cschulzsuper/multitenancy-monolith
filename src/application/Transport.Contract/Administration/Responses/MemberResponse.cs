namespace ChristianSchulz.MultitenancyMonolith.Application.Administration.Responses;

public class MemberResponse
{
    public required string UniqueName { get; init; }

    public required string Group { get; init; }

    public required string Identity { get; init; }
}