using System.Collections.Concurrent;

namespace ChristianSchulz.MultitenancyMonolith.Data;

internal sealed class RepositoryContext<TEntity>
{
    public ConcurrentDictionary<object, TEntity> Data { get; } = new ConcurrentDictionary<object, TEntity>();

    public Func<TEntity, object> SnowflakeFactory { get; set; } = _ => Guid.NewGuid();

    public Func<TEntity, object> SnowflakeProvider { get; set; } = _ => Guid.Empty;
}
