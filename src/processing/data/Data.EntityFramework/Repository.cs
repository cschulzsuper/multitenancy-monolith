using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Data.EntityFramework;

public sealed class Repository<TEntity> : IRepository<TEntity>
    where TEntity : class
{
    private readonly DbContext _context;

    private static string _snowflakePropertyName = null!;
    private static PropertyInfo _snowflakeProperty = null!;

    public Repository(DbContext context)
    {
        _context = context;
        _context.Database.EnsureCreated();

        _snowflakePropertyName ??= _context.Model
            .FindEntityType(typeof(TEntity))?
            .FindPrimaryKey()?.Properties
            .Select(x => x.Name).Single()
            ?? throw new UnreachableException("Snowflake property name not found.");

        _snowflakeProperty ??= typeof(TEntity).GetProperty(_snowflakePropertyName)
            ?? throw new UnreachableException("Snowflake property not found.");
    }

    private object GetSnowflake(TEntity entity)
    {
        var snowflake = _snowflakeProperty.GetValue(entity, null)
            ?? throw new UnreachableException("Snowflake not found.");

        return snowflake;
    }

    private static Expression<Func<TEntity, bool>> GetSnowflakePredicate(object snowflake)
    {
        var parameterExpression = Expression.Parameter(typeof(TEntity), "entity");
        var propertyExpression = Expression.Property(parameterExpression, _snowflakePropertyName);
        var valueExpression = Expression.Constant(snowflake);
        var equalityExpression = Expression.Equal(propertyExpression, valueExpression);
        var predicateExpression = Expression.Lambda<Func<TEntity, bool>>(equalityExpression, parameterExpression);

        return predicateExpression;
    }

    private static Expression<Func<TEntity, TEntity>> GetSnowflakeObject()
    {
        var parameterExpression = Expression.Parameter(typeof(TEntity), "entity");
        var propertyExpression = Expression.Property(parameterExpression, _snowflakePropertyName);
        var newExpression = Expression.New(typeof(TEntity));
        var propertyBinding = Expression.Bind(_snowflakeProperty, propertyExpression);
        var propertyInitExpression = Expression.MemberInit(newExpression, propertyBinding);
        var lambdaExpression = Expression.Lambda<Func<TEntity, TEntity>>(propertyInitExpression, parameterExpression);

        return lambdaExpression;
    }

    public void Execute(Action<IRepository<TEntity>> action)
    {
        using var transaction = _context.Database.BeginTransaction();

        try
        {
            action.Invoke(this);

            transaction.Commit();
        }
        catch
        {
            RepositoryException.ThrowTransactionFailed();
        }
    }

    public async Task ExecuteAsync(Func<IRepository<TEntity>, Task> func)
    {
        using var transaction = _context.Database.BeginTransaction();

        await func.Invoke(this);

        try
        {
            await transaction.CommitAsync();
        }
        catch (OperationCanceledException)
        {
            RepositoryException.ThrowTransactionFailed();
        }
    }

    public bool Exists(object snowflake)
    {
        var entity = _context.Set<TEntity>()
            .SingleOrDefault(GetSnowflakePredicate(snowflake));

        return entity != null;
    }


    public bool Exists(Expression<Func<TEntity, bool>> predicate)
    {
        var entity = _context.Set<TEntity>()
            .SingleOrDefault(predicate);

        return entity != null;
    }

    public async Task<bool> ExistsAsync(object snowflake)
    {
        var entity = await _context.Set<TEntity>()
            .SingleOrDefaultAsync(GetSnowflakePredicate(snowflake));

        return entity != null;
    }

    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var entity = await _context.Set<TEntity>()
            .SingleOrDefaultAsync(predicate);

        return entity != null;
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

    public IQueryable<TEntity> GetQueryable(FormattableString query)
        => _context.Set<TEntity>().FromSqlInterpolated(query);

    public IAsyncEnumerable<TEntity> GetAsyncEnumerable(CancellationToken cancellationToken = default)
        => _context.Set<TEntity>().AsAsyncEnumerable();

    public IAsyncEnumerable<TEntity> GetAsyncEnumerable(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => _context.Set<TEntity>().Where(predicate).AsAsyncEnumerable();

    public IAsyncEnumerable<TResult> GetAsyncEnumerable<TResult>(Func<IQueryable<TEntity>, IQueryable<TResult>> query, CancellationToken cancellationToken = default)
        => query.Invoke(_context.Set<TEntity>()).AsAsyncEnumerable();

    public void Insert(TEntity entity)
        => Insert([entity]);

    public void Insert(params TEntity[] entities)
        => Insert(entities as ICollection<TEntity>);

    public void Insert(ICollection<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            _context.Set<TEntity>().Add(entity);
        }

        _context.SaveChanges();
    }

    public Task InsertAsync(TEntity entity)
        => InsertAsync([entity]);

    public Task InsertAsync(params TEntity[] entities)
        => InsertAsync(entities as ICollection<TEntity>);

    public async Task InsertAsync(ICollection<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            await _context.Set<TEntity>().AddAsync(entity);
        }

        await _context.SaveChangesAsync();
    }

    public int Update(object snowflake, Action<TEntity> action)
    {
        var entity = _context.Set<TEntity>().Find(snowflake);
        if (entity != null)
        {
            action.Invoke(entity);
            _context.Set<TEntity>().Update(entity);

            return _context.SaveChanges();
        }

        return 0;
    }

    public ICollection<object> Update(Expression<Func<TEntity, bool>> predicate, Action<TEntity> action)
    {
        var entities = GetQueryable()
            .Where(predicate)
            .ToArray();

        foreach (var entity in entities)
        {
            action.Invoke(entity);
            _context.Set<TEntity>().Update(entity);
        }

        _context.SaveChanges();

        var snowflakes = entities.Select(GetSnowflake).ToArray();
        return snowflakes;
    }

    public async Task<int> UpdateAsync(object snowflake, Action<TEntity> action)
    {
        var entity = _context.Set<TEntity>().Find(snowflake);
        if (entity != null)
        {
            action.Invoke(entity);
            _context.Set<TEntity>().Update(entity);

            return await _context.SaveChangesAsync();
        }

        return 0;
    }

    public async Task<ICollection<object>> UpdateAsync(Expression<Func<TEntity, bool>> predicate, Action<TEntity> action)
    {
        var entities = GetQueryable()
            .Where(predicate)
            .ToArray();

        foreach (var entity in entities)
        {
            action.Invoke(entity);
            _context.Set<TEntity>().Update(entity);
        }

        await _context.SaveChangesAsync();

        var snowflakes = entities.Select(GetSnowflake).ToArray();
        return snowflakes;
    }

    public int Delete(object snowflake)
    {
        var entity = _context.Set<TEntity>()
            .Where(GetSnowflakePredicate(snowflake))
            .Select(GetSnowflakeObject())
            .SingleOrDefault();

        if (entity != null)
        {
            _context.Set<TEntity>().Remove(entity);

            return _context.SaveChanges();
        }

        return 0;
    }

    public ICollection<object> Delete(Expression<Func<TEntity, bool>> predicate)
    {
        var entities = GetQueryable()
            .Where(predicate)
            .ToArray();

        _context.Set<TEntity>().Where(predicate).ExecuteDelete();

        var snowflakes = entities.Select(GetSnowflake).ToArray();
        return snowflakes;
    }

    public async Task<int> DeleteAsync(object snowflake)
    {
        var entity = await _context.Set<TEntity>()
            .Where(GetSnowflakePredicate(snowflake))
            .Select(GetSnowflakeObject())
            .SingleOrDefaultAsync();

        if (entity != null)
        {
            _context.Set<TEntity>().Remove(entity);

            return await _context.SaveChangesAsync();
        }

        return 0;
    }

    public async Task<ICollection<object>> DeleteAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var entities = await GetQueryable()
            .Where(predicate)
            .ToArrayAsync();

        await _context.Set<TEntity>().Where(predicate).ExecuteDeleteAsync();

        var snowflakes = entities.Select(GetSnowflake).ToArray();
        return snowflakes;
    }
}