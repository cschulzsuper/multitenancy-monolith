using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Data.EntityFramework.Sqlite;

public abstract class Repository<TEntity> : IRepository<TEntity>
    where TEntity : class, ICloneable
{
    private readonly DbContext _context;

    public Repository(DbContext context)
    {
        _context = context;
    }

    public void Execute(Action<IRepository<TEntity>> action)
    {
        throw new NotImplementedException();
    }

    public Task ExecuteAsync(Func<IRepository<TEntity>, Task> func)
    {
        throw new NotImplementedException();
    }

    public bool Exists(object snowflake)
    {
        throw new NotImplementedException();
    }

    public bool Exists(Expression<Func<TEntity, bool>> predicate)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(object snowflake)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
    {
        throw new NotImplementedException();
    }

    public TEntity? GetOrDefault(object snowflake)
    => _context.Set<TEntity>().Find(snowflake);

    public TEntity? GetOrDefault(Expression<Func<TEntity, bool>> predicate)
        => _context.Set<TEntity>().SingleOrDefault(predicate);

    public Task<TEntity?> GetOrDefaultAsync(object snowflake)
        => _context.Set<TEntity>().FindAsync(snowflake).AsTask();

    public Task<TEntity?> GetOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        => _context.Set<TEntity>().SingleOrDefaultAsync(predicate);

    public IQueryable<TEntity> GetQueryable()
    => _context.Set<TEntity>();

    public abstract IQueryable<TEntity> GetQueryable(FormattableString query);

    public IAsyncEnumerable<TEntity> GetAsyncEnumerable(CancellationToken cancellationToken = default)
        => _context.Set<TEntity>().AsAsyncEnumerable();

    public IAsyncEnumerable<TEntity> GetAsyncEnumerable(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => _context.Set<TEntity>().Where(predicate).AsAsyncEnumerable();

    public IAsyncEnumerable<TResult> GetAsyncEnumerable<TResult>(Func<IQueryable<TEntity>, IQueryable<TResult>> query, CancellationToken cancellationToken = default)
        => query.Invoke(_context.Set<TEntity>()).AsAsyncEnumerable();

    public void Insert(TEntity entity)
    {
        throw new NotImplementedException();
    }

    public void Insert(params TEntity[] entities)
    {
        throw new NotImplementedException();
    }

    public void Insert(ICollection<TEntity> entities)
    {
        throw new NotImplementedException();
    }

    public Task InsertAsync(TEntity entity)
    {
        throw new NotImplementedException();
    }

    public Task InsertAsync(params TEntity[] entities)
    {
        throw new NotImplementedException();
    }

    public Task InsertAsync(ICollection<TEntity> entities)
    {
        throw new NotImplementedException();
    }

    public int Update(object snowflake, Action<TEntity> action)
    {
        throw new NotImplementedException();
    }

    public ICollection<object> Update(Expression<Func<TEntity, bool>> predicate, Action<TEntity> action)
    {
        throw new NotImplementedException();
    }

    public Task<int> UpdateAsync(object snowflake, Action<TEntity> action)
    {
        throw new NotImplementedException();
    }

    public Task<ICollection<object>> UpdateAsync(Expression<Func<TEntity, bool>> predicate, Action<TEntity> action)
    {
        throw new NotImplementedException();
    }

    public int Delete(object snowflake)
    {
        throw new NotImplementedException();
    }

    public ICollection<object> Delete(Expression<Func<TEntity, bool>> predicate)
    {
        throw new NotImplementedException();
    }

    public Task<int> DeleteAsync(object snowflake)
    {
        throw new NotImplementedException();
    }

    public Task<ICollection<object>> DeleteAsync(Expression<Func<TEntity, bool>> predicate)
    {
        throw new NotImplementedException();
    }
}