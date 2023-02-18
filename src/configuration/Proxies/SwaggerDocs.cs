namespace ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;

public sealed class SwaggerDocs
{
    public required string DisplayName { get; init; }

    public required string Host { get; init; }

    public required string Path { get; init; }
}