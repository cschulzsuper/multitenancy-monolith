namespace ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;

public class DistributedCache
{
    public required string Host { get; init; }

    public required string Secret { get; init; }
}
