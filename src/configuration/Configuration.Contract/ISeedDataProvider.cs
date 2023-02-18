namespace ChristianSchulz.MultitenancyMonolith.Configuration;

public interface ISeedDataProvider
{
    ICollection<TEntity> Get<TEntity>(string category, string key);

    IDictionary<string, ICollection<TEntity>> GetGrouped<TEntity>(string category, string key);
}
