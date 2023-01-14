namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

public sealed class IdentityVerficationKey
{
    public required string Client { get; init; }

    public required string Identity { get; init; }
}
