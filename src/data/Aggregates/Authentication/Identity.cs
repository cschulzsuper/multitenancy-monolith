namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

public class Identity
{
    public required string UniqueName { get; set; }
    public required string Secret { get; set; }

    public byte[] Verification { get; set; } = Guid.NewGuid().ToByteArray();
}