using Microsoft.Extensions.Configuration;

namespace ChristianSchulz.MultitenancyMonolith.Configuration;

public sealed class SeedDataProvider : ISeedDataProvider
{
    private readonly IConfiguration _configuration;

    public SeedDataProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public ICollection<TEntity> Get<TEntity>(string category, string key)
        => _configuration.GetSection($"SeedData:{category}:{key}").Get<TEntity[]>() ?? Array.Empty<TEntity>();

    public IDictionary<string, ICollection<TEntity>> GetGrouped<TEntity>(string category, string key)
    {
        var groupSections = _configuration.GetSection($"SeedData:{category}:{key}")?.GetChildren();
        if (groupSections == null)
        {
            return new Dictionary<string, ICollection<TEntity>>();
        }

        var grouped = groupSections
            .ToDictionary(
                group => group.Key,
                group => (ICollection<TEntity>)(group.Get<TEntity[]>() ?? Array.Empty<TEntity>()));

        return grouped;
    }
}