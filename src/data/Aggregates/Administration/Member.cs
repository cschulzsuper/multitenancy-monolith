namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public class Member
{
    public required string UniqueName { get; set; }

    public required string Group { get; set; }

    public required string Identity { get; set; }
}