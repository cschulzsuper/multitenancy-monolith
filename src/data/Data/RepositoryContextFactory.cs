using System.Collections.Concurrent;

namespace ChristianSchulz.MultitenancyMonolith.Data;

internal sealed class RepositoryContextFactory<TEntity>
{
    private readonly ConcurrentDictionary<string,RepositoryContext<TEntity>> _contexts = new();

    public RepositoryContext<TEntity> Create()
        => Create(string.Empty);

    public RepositoryContext<TEntity> Create(Func<TEntity, object> snowflakeFactory)
        => Create(string.Empty, snowflakeFactory);

    public RepositoryContext<TEntity> Create(string multitenancyDiscirmantor)
    {
        var repositoryContext = _contexts.GetOrAdd(multitenancyDiscirmantor, new RepositoryContext<TEntity>());

        return repositoryContext;
    }

    public RepositoryContext<TEntity> Create(string multitenancyDiscirmantor, Func<TEntity, object> snowflakeFactory)
    {
        var repositoryContext = Create(multitenancyDiscirmantor);

        repositoryContext.SnowflakeFactory = snowflakeFactory;

        return repositoryContext;
    }
}
