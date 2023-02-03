using System.Collections.Concurrent;

namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary;

internal sealed class RepositoryContextFactory<TEntity>
{
    private readonly ConcurrentDictionary<string, RepositoryContext<TEntity>> _contexts = new();

    public RepositoryContext<TEntity> Create()
        => Create(string.Empty);

    public RepositoryContext<TEntity> Create(
        Func<TEntity, object> snowflakeFactory,
        Func<TEntity, object> snowflakeProvider,
        Action<IEnumerable<TEntity>, TEntity> ensurance)
        => Create(string.Empty, snowflakeFactory, snowflakeProvider, ensurance);

    public RepositoryContext<TEntity> Create(string multitenancyDiscriminator)
    {
        var repositoryContext = _contexts.GetOrAdd(multitenancyDiscriminator, new RepositoryContext<TEntity>());

        return repositoryContext;
    }

    public RepositoryContext<TEntity> Create(string multitenancyDiscriminator,
        Func<TEntity, object> snowflakeFactory,
        Func<TEntity, object> snowflakeProvider,
        Action<IEnumerable<TEntity>, TEntity> ensurance)
    {
        var repositoryContext = Create(multitenancyDiscriminator);

        repositoryContext.SnowflakeFactory = snowflakeFactory;
        repositoryContext.SnowflakeProvider = snowflakeProvider;
        repositoryContext.Ensurance = ensurance;

        return repositoryContext;
    }
}