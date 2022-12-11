namespace ChristianSchulz.MultitenancyMonolith.Data;

public interface IRepository<TEntity>
{
    TEntity Get(object snowflake);

    TEntity? GetOrDefault(object snowflake);

    IQueryable<TEntity> GetQueryable();
    
    void Insert(TEntity entity);
    
    void InsertMany(ICollection<TEntity> entities);
}
