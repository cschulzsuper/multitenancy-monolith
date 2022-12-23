using System.Linq.Expressions;

namespace ChristianSchulz.MultitenancyMonolith.Data;

public interface IRepository<TEntity>
{
    TEntity Get(object snowflake);

    TEntity Get(Expression<Func<TEntity, bool>> predicate);

    ValueTask<TEntity> GetAsync(object snowflake);

    ValueTask<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate);

    IQueryable<TEntity> GetQueryable();

    IQueryable<TEntity> GetQueryable(FormattableString query);

    IAsyncEnumerable<TEntity> GetAsyncEnumerable(CancellationToken cancellationToken = default);

    IAsyncEnumerable<TEntity> GetAsyncEnumerable(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    IAsyncEnumerable<TResult> GetAsyncEnumerable<TResult>(Func<IQueryable<TEntity>, IQueryable<TResult>> query, CancellationToken cancellationToken = default);

    void Insert(TEntity entity);

    void Insert(params TEntity[] entities);

    void Insert(ICollection<TEntity> entities);

    ValueTask InsertAsync(TEntity entity);

    ValueTask InsertAsync(params TEntity[] entities);

    ValueTask InsertAsync(ICollection<TEntity> entities);

    int Delete(object snowflake);

    int Delete(TEntity entity);

    int Delete(params TEntity[] entities);

    int Delete(ICollection<TEntity> entities);

    int Delete(Expression<Func<TEntity, bool>> query);

    ValueTask<int> DeleteAsync(object snowflake);

    ValueTask<int> DeleteAsync(TEntity entity);

    ValueTask<int> DeleteAsync(params TEntity[] entities);

    ValueTask<int> DeleteAsync(ICollection<TEntity> entities);

    ValueTask<int> DeleteAsync(Expression<Func<TEntity, bool>> query);
}
