using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Data;

public interface IRepository<TEntity>
{
    void Execute(Action<IRepository<TEntity>> action);

    ValueTask ExecuteAsync(Func<IRepository<TEntity>, ValueTask> func);

    bool Exists(object snowflake);

    bool Exists(Expression<Func<TEntity, bool>> predicate);

    ValueTask<bool> ExistsAsync(object snowflake);

    ValueTask<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);

    TEntity? GetOrDefault(object snowflake);

    TEntity? GetOrDefault(Expression<Func<TEntity, bool>> predicate);

    ValueTask<TEntity?> GetOrDefaultAsync(object snowflake);

    ValueTask<TEntity?> GetOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

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