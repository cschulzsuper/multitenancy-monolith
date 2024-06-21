using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary;

internal sealed class Repository<TEntity> : IRepository<TEntity>
    where TEntity : class, ICloneable
{
    private readonly RepositoryContext<TEntity> _context;

    public Repository(RepositoryContext<TEntity> context)
    {
        _context = context;
    }

    public void Execute(Action<IRepository<TEntity>> action)
    {
        using var _ = _context.AcquireLock();

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

    public async Task ExecuteAsync(Func<IRepository<TEntity>, Task> func)
    {
        using var _ = await _context.AcquireLockAsync();

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

    public bool Exists(object snowflake)
    {
        var found = _context.Data.ContainsKey(snowflake);

        return found;
    }

    public bool Exists(Expression<Func<TEntity, bool>> predicate)
    {
        var entity = _context.Data.Values
            .SingleOrDefault(entity => predicate.Compile().Invoke(entity));

        return entity != null;
    }

    public Task<bool> ExistsAsync(object snowflake)
    {
        var entity = Exists(snowflake);
        return Task.FromResult(entity);
    }

    public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var entity = Exists(predicate);
        return Task.FromResult(entity);
    }

    public TEntity? GetOrDefault(object snowflake)
    {
        _ = _context.Data.TryGetValue(snowflake, out var entity);

        return entity;
    }

    public TEntity? GetOrDefault(Expression<Func<TEntity, bool>> predicate)
    {
        var entity = _context.Data.Values
            .SingleOrDefault(entity => predicate.Compile().Invoke(entity));

        return entity;
    }

    public Task<TEntity?> GetOrDefaultAsync(object snowflake)
    {
        var entity = GetOrDefault(snowflake);
        return Task.FromResult(entity);
    }

    public Task<TEntity?> GetOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var entity = GetOrDefault(predicate);
        return Task.FromResult(entity);
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
        await Task.CompletedTask;

        var entities = _context.Data.Values;

        foreach (var entity in entities)
        {
            yield return entity;
        }
    }

    public async IAsyncEnumerable<TEntity> GetAsyncEnumerable(Expression<Func<TEntity, bool>> predicate,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;

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
        await Task.CompletedTask;

        var entities = query
            .Invoke(_context.Data.Values.AsQueryable())
            .ToArray();

        foreach (var entity in entities)
        {
            yield return entity;
        }
    }

    private void EnsureInsertable(TEntity entity)
    {
        var values = _context.Data.Values;

        _context.Ensurance.Invoke(values, entity);
    }

    private void EnsureUpdatable(object snowflake, TEntity entity)
    {
        var values = _context.Data
            .Where(x => !x.Key.Equals(snowflake))
            .Select(x => x.Value);

        _context.Ensurance.Invoke(values, entity);
    }

    public void Insert(TEntity entity)
    {
        var snowflake = _context.SnowflakeFactory(entity);

        _context.Data.AddOrUpdate(snowflake,
            _ =>
            {
                EnsureInsertable(entity);
                var inserted = (TEntity)entity!.Clone();
                return inserted;
            },
            (snowflake, entity) =>
            {
                DataException.ThrowObjectConflict<TEntity>(snowflake);
                return entity;
            });
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

    public Task InsertAsync(TEntity entity)
    {
        Insert(entity);
        return Task.CompletedTask;
    }

    public Task InsertAsync(params TEntity[] entities)
    {
        Insert(entities);
        return Task.CompletedTask;
    }

    public Task InsertAsync(ICollection<TEntity> entities)
    {
        Insert(entities);
        return Task.CompletedTask;
    }

    public int Update(object snowflake, Action<TEntity> action)
    {
        var found = _context.Data.TryGetValue(snowflake, out var entity);

        if (found)
        {
            _context.Data.AddOrUpdate(snowflake,
                _ => throw new UnreachableException("Update caused insert."),
                (_, _) =>
                {
                    var updated = (TEntity)entity!.Clone();
                    action(updated);
                    EnsureUpdatable(snowflake, updated);

                    return updated;
                });

            return 1;
        }

        return 0;
    }

    public ICollection<object> Update(Expression<Func<TEntity, bool>> predicate, Action<TEntity> action)
    {
        var entities = GetQueryable()
            .Where(predicate)
            .ToArray();

        var snowflakes = entities
            .Select(entity => _context.SnowflakeProvider(entity))
            .Where(snowflake =>
            {
                return Update(snowflake, action) == 1;
            })
            .ToImmutableArray();

        return snowflakes;
    }

    public Task<int> UpdateAsync(object snowflake, Action<TEntity> action)
    {
        var affectedRows = Update(snowflake, action);
        return Task.FromResult(affectedRows);
    }

    public Task<ICollection<object>> UpdateAsync(Expression<Func<TEntity, bool>> predicate, Action<TEntity> action)
    {
        var affectedRows = Update(predicate, action);
        return Task.FromResult(affectedRows);
    }

    public int Delete(object snowflake)
    {
        var removed = _context.Data.TryRemove(snowflake, out _);
        return removed ? 1 : 0;
    }

    public ICollection<object> Delete(Expression<Func<TEntity, bool>> predicate)
    {
        var entities = GetQueryable()
            .Where(predicate)
            .ToArray();

        var snowflakes = entities
            .Select(entity => _context.SnowflakeProvider(entity))
            .Where(snowflake =>
            {
                return Delete(snowflake) == 1;
            })
            .ToImmutableArray();

        return snowflakes;
    }

    public Task<int> DeleteAsync(object snowflake)
    {
        var affectedRows = Delete(snowflake);
        return Task.FromResult(affectedRows);
    }

    public Task<ICollection<object>> DeleteAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var affectedRows = Delete(predicate);
        return Task.FromResult(affectedRows);
    }
}