namespace ChristianSchulz.MultitenancyMonolith.Data;

public interface IRepository<TEntity>
{
    TEntity Get(object id);

    TEntity? GetOrDefault(object id);

    IQueryable<TEntity> GetQueryable();
}
