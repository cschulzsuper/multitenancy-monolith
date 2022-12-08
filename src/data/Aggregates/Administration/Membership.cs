namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public class Membership
{
    public required string Identity { get; set; }
    public required string Group { get; set; }
    public required string Member { get; set; }
}
