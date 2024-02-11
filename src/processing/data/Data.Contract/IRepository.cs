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

    Task ExecuteAsync(Func<IRepository<TEntity>, Task> func);

    bool Exists(object snowflake);

    bool Exists(Expression<Func<TEntity, bool>> predicate);

    Task<bool> ExistsAsync(object snowflake);

    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);

    TEntity? GetOrDefault(object snowflake);

    TEntity? GetOrDefault(Expression<Func<TEntity, bool>> predicate);

    Task<TEntity?> GetOrDefaultAsync(object snowflake);

    Task<TEntity?> GetOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

    IQueryable<TEntity> GetQueryable();

    IQueryable<TEntity> GetQueryable(FormattableString query);

    IAsyncEnumerable<TEntity> GetAsyncEnumerable(CancellationToken cancellationToken = default);

    IAsyncEnumerable<TEntity> GetAsyncEnumerable(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    IAsyncEnumerable<TResult> GetAsyncEnumerable<TResult>(Func<IQueryable<TEntity>, IQueryable<TResult>> query, CancellationToken cancellationToken = default);

    void Insert(TEntity entity);

    void Insert(params TEntity[] entities);

    void Insert(ICollection<TEntity> entities);

    Task InsertAsync(TEntity entity);

    Task InsertAsync(params TEntity[] entities);

    Task InsertAsync(ICollection<TEntity> entities);

    int Update(object snowflake, Action<TEntity> action);

    ICollection<object> Update(Expression<Func<TEntity, bool>> predicate, Action<TEntity> action);

    Task<int> UpdateAsync(object snowflake, Action<TEntity> action);

    Task<ICollection<object>> UpdateAsync(Expression<Func<TEntity, bool>> predicate, Action<TEntity> action);

    int Delete(object snowflake);

    ICollection<object> Delete(Expression<Func<TEntity, bool>> predicate);

    Task<int> DeleteAsync(object snowflake);

    Task<ICollection<object>> DeleteAsync(Expression<Func<TEntity, bool>> predicate);
}