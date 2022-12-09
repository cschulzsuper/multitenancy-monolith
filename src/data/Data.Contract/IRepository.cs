namespace ChristianSchulz.MultitenancyMonolith.Data;

public interface IRepository<TEntity>
{
    TEntity Get(object snowflake);

    TEntity? GetOrDefault(object snowflake);

    IQueryable<TEntity> GetQueryable();

    void InsertMany(ICollection<TEntity> entities);
}
