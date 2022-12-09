namespace ChristianSchulz.MultitenancyMonolith.Data;

internal sealed class RepositoryContext<TEntity>
{
    public IDictionary<object, TEntity> Data { get; } = new Dictionary<object, TEntity>();

    public Func<TEntity,object> SnowflakeFactory { get; set;  } = _ => Guid.NewGuid();
}
