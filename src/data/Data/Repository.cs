using System.Reflection.Metadata.Ecma335;

namespace ChristianSchulz.MultitenancyMonolith.Data;

internal sealed class Repository<TEntity> : IRepository<TEntity>
{
    private readonly RepositoryContext<TEntity> _context;

    public Repository(RepositoryContext<TEntity> context)
    {
        _context = context;
    }

    public TEntity Get(object snowflake)
    {
        var found = _context.Data.TryGetValue(snowflake, out var entity);

        if (!found)
        {
            throw new RepositoryException($"{typeof(TEntity).Name} `{snowflake}` does not exist");
        }

        return entity!;
    }

    public TEntity? GetOrDefault(object snowflake)
    {
        _context.Data.TryGetValue(snowflake, out var entity);
        return entity;
    }

    public IQueryable<TEntity> GetQueryable()
        => _context.Data.Values.AsQueryable();

    public void InsertMany(ICollection<TEntity> entities)
    {
        foreach(var entity in entities)
        {
            var snowflake = _context.SnowflakeFactory(entity);

            _context.Data.Add(snowflake, entity);
        }
    }
}
