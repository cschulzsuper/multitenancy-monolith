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
            foreach (var entry in backup)
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

    public int Update(object snowflake, Action<TEntity> action)
    {
        var found = _context.Data.TryGetValue(snowflake, out var entity);

        if (found)
        {
            if (entity is ICloneable cloneable)
            {
                var updated = (TEntity)cloneable.Clone();
                action(updated);
                _context.Data.TryUpdate(snowflake, updated, entity);
            }
            else
            {
                action(entity!);
            }

            return 1;
        }

        return 0;
    }

    public int Update(Expression<Func<TEntity, bool>> predicate, Action<TEntity> action)
    {
        var entities = GetQueryable()
            .Where(predicate)
            .ToArray();

        var rowsAffected = entities
            .Sum(entity =>
            {
                var snowflake = _context.SnowflakeProvider(entity);
                return Update(snowflake, action);
            });

        return rowsAffected;
    }

    public ValueTask<int> UpdateAsync(object snowflake, Action<TEntity> action)
    {
        var rowsAffected = Update(snowflake, action);
        return ValueTask.FromResult(rowsAffected);
    }

    public ValueTask<int> UpdateAsync(Expression<Func<TEntity, bool>> predicate, Action<TEntity> action)
    {
        var rowsAffected = Update(predicate, action);
        return ValueTask.FromResult(rowsAffected);
    }

    public int Delete(object snowflake)
    {
        var removed = _context.Data.TryRemove(snowflake, out _);
        return removed ? 1 : 0;
    }

    public int Delete(Expression<Func<TEntity, bool>> predicate)
    {
        var entities = GetQueryable()
            .Where(predicate)
            .ToArray();

        var rowsAffected = entities
            .Sum(entity =>
            {
                var snowflake = _context.SnowflakeProvider(entity);
                return Delete(snowflake);
            });

        return rowsAffected;
    }

    public ValueTask<int> DeleteAsync(object snowflake)
    {
        var rowsAffected = Delete(snowflake);
        return ValueTask.FromResult(rowsAffected);
    }

    public ValueTask<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var rowsAffected = Delete(predicate);
        return ValueTask.FromResult(rowsAffected);
    }
}