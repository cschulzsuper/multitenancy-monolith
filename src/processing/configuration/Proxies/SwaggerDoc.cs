namespace ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;

public sealed class SwaggerDoc
{
    public required string DisplayName { get; init; }

    public required string TestService{ get; init; }

    public required string PublicService { get; init; }

    public required string Path { get; init; }
}