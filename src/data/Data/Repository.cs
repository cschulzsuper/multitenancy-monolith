namespace ChristianSchulz.MultitenancyMonolith.Data;

internal sealed class Repository<TEntity> : IRepository<TEntity>
{
    private readonly IDictionary<object, TEntity> _entities;

    public Repository(IDictionary<object, TEntity> entities)
    {
        _entities = entities;
    }

    public TEntity Get(object id)
    {
        var found = _entities.TryGetValue(id, out var entity);

        if (!found)
        {
            throw new RepositoryException($"{typeof(TEntity).Name} `{id}` does not exist");
        }

        return entity!;
    }

    public TEntity? GetOrDefault(object id)
    {
        _entities.TryGetValue(id, out var entity);
        return entity;
    }

    public IQueryable<TEntity> GetQueryable()
        => _entities.Values.AsQueryable();
}
