using System.Linq.Expressions;

namespace ChristianSchulz.MultitenancyMonolith.Data;

public interface IRepository<TEntity>
{
    void Execute(Action<IRepository<TEntity>> action);
    
    ValueTask ExecuteAsync(Func<IRepository<TEntity>, ValueTask> func);

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

    int Update(object snowflake, Action<TEntity> action);

    int Update(Expression<Func<TEntity, bool>> predicate, Action<TEntity> action);

    ValueTask<int> UpdateAsync(object snowflake, Action<TEntity> action);

    ValueTask<int> UpdateAsync(Expression<Func<TEntity, bool>> predicate, Action<TEntity> action);

    int Delete(object snowflake);

    int Delete(Expression<Func<TEntity, bool>> predicate);

    ValueTask<int> DeleteAsync(object snowflake);

    ValueTask<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate);
}