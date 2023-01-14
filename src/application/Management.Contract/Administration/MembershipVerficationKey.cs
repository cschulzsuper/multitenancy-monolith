namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public sealed class MembershipVerficationKey
{
    public required string Client { get; init; }
    
    public required string Group { get; init; }

    public required string Member { get; init; }
}
