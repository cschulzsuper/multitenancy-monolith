using ChristianSchulz.MultitenancyMonolith.Aggregates.Authentication;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace ChristianSchulz.MultitenancyMonolith.Data;

internal sealed class Repository<TEntity> : IRepository<TEntity>
{
    private readonly RepositoryContext<TEntity> _context;

    public Repository(RepositoryContext<TEntity> context)
    {
        _context = context;
    }

    public void Execute(Action<IRepository<TEntity>> action)
    {
        using var _ = _context.AquireLock();

        var backup = _context.Data.ToDictionary(
            x => x.Key,
            x => x.Value);

        try
        {
            action.Invoke(this);
        }
        catch
        {
            _context.Data.Clear();
            foreach(var entry in backup)
            {
                _context.Data.TryAdd(entry.Key, entry.Value);
            }

            throw;
        }
    }

    public async ValueTask ExecuteAsync(Func<IRepository<TEntity>, ValueTask> func)
    {
        using var _ = _context.AquireLock();

        var backup = _context.Data.ToDictionary(
            x => x.Key,
            x => x.Value);

        try
        {
            await func.Invoke(this);
        }
        catch
        {
            _context.Data.Clear();
            foreach (var entry in backup)
            {
                _context.Data.TryAdd(entry.Key, entry.Value);
            }

            throw;
        }
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

    public TEntity Get(Expression<Func<TEntity, bool>> predicate)
    {
        var entity = _context.Data.Values
            .SingleOrDefault(entity => predicate.Compile().Invoke(entity));

        if (entity == null)
        {
            throw new RepositoryException($"Single entity of type '{typeof(TEntity).Name}' does not exist");
        }

        return entity!;
    }

    public ValueTask<TEntity> GetAsync(object snowflake)
    {
        var entity = Get(snowflake);
        return ValueTask.FromResult(entity);
    }

    public ValueTask<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var entity = Get(predicate);
        return ValueTask.FromResult(entity);
    }

    public IQueryable<TEntity> GetQueryable()
        => _context.Data.Values.AsQueryable();

    public IQueryable<TEntity> GetQueryable(FormattableString query)
    {
        throw new NotSupportedException("String queries are not supported by the static dictionary implementation.");
    }

    public async IAsyncEnumerable<TEntity> GetAsyncEnumerable(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await ValueTask.CompletedTask;

        var entities = _context.Data.Values;

        foreach (var entity in entities)
        {
            yield return entity;
        }
    }

    public async IAsyncEnumerable<TEntity> GetAsyncEnumerable(Expression<Func<TEntity, bool>> predicate,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await ValueTask.CompletedTask;

        var entities = _context.Data.Values
            .Where(predicate.Compile())
            .ToArray();

        foreach (var entity in entities)
        {
            yield return entity;
        }
    }

    public async IAsyncEnumerable<TResult> GetAsyncEnumerable<TResult>(Func<IQueryable<TEntity>, IQueryable<TResult>> query,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await ValueTask.CompletedTask;

        var entities = query
            .Invoke(_context.Data.Values.AsQueryable())
            .ToArray();

        foreach (var entity in entities)
        {
            yield return entity;
        }
    }

    public void Insert(TEntity entity)
    {
        var snowflake = _context.SnowflakeFactory(entity);

        _context.Data.TryAdd(snowflake, entity);
    }

    public void Insert(params TEntity[] entities)
        => Insert(entities as ICollection<TEntity>);

    public void Insert(ICollection<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            Insert(entity);
        }
    }

    public ValueTask InsertAsync(TEntity entity)
    {
        Insert(entity);
        return ValueTask.CompletedTask;
    }

    public ValueTask InsertAsync(params TEntity[] entities)
    {
        Insert(entities);
        return ValueTask.CompletedTask;
    }

    public ValueTask InsertAsync(ICollection<TEntity> entities)
    {
        Insert(entities);
        return ValueTask.CompletedTask;
    }

    public int Delete(object snowflake)
    {
        var removed = _context.Data.TryRemove(snowflake, out _);
        return removed ? 1 : 0;
    }

    public int Delete(TEntity entity)
    {
        var snowflake = _context.SnowflakeProvider(entity);

        var rowsAffected = Delete(snowflake);
        return rowsAffected;
    }

    public int Delete(params TEntity[] entities)
        => Delete(entities as ICollection<TEntity>);

    public int Delete(ICollection<TEntity> entities)
    {
        var rowsAffected = entities.Sum(Delete);
        return rowsAffected;
    }

    public int Delete(Expression<Func<TEntity, bool>> query)
    {
        var entities = GetQueryable()
            .Where(query)
            .ToArray();

        var rowsAffected = Delete(entities);
        return rowsAffected;
    }

    public ValueTask<int> DeleteAsync(object snowflake)
    {
        var rowsAffected = Delete(snowflake);
        return ValueTask.FromResult(rowsAffected);
    }

    public ValueTask<int> DeleteAsync(TEntity entity)
    {
        var rowsAffected = Delete(entity);
        return ValueTask.FromResult(rowsAffected);
    }

    public ValueTask<int> DeleteAsync(params TEntity[] entities)
    {
        var rowsAffected = Delete(entities);
        return ValueTask.FromResult(rowsAffected);
    }

    public ValueTask<int> DeleteAsync(ICollection<TEntity> entities)
    {
        var rowsAffected = Delete(entities);
        return ValueTask.FromResult(rowsAffected);
    }

    public ValueTask<int> DeleteAsync(Expression<Func<TEntity, bool>> query)
    {
        var rowsAffected = Delete(query);
        return ValueTask.FromResult(rowsAffected);
    }
}
