using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Data;

public static class RepositoryExtensions
{
    public static TEntity Get<TEntity>(this IRepository<TEntity> repository, object snowflake)
    {
        var entity = repository.GetOrDefault(snowflake);

        if (entity == null)
        {
            RepositoryException.ThrowObjectNotFound<TEntity>(snowflake);
        }

        return entity;
    }

    public static TEntity Get<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate)
    {
        var entity = repository.GetOrDefault(predicate);

        if (entity == null)
        {
            RepositoryException.ThrowObjectNotFound<TEntity>();
        }

        return entity;
    }

    public static async Task<TEntity> GetAsync<TEntity>(this IRepository<TEntity> repository, object snowflake)
    {
        var entity = await repository.GetOrDefaultAsync(snowflake);

        if (entity == null)
        {
            RepositoryException.ThrowObjectNotFound<TEntity>(snowflake);
        }

        return entity;
    }

    public static async Task<TEntity> GetAsync<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate)
    {
        var entity = await repository.GetOrDefaultAsync(predicate);

        if (entity == null)
        {
            RepositoryException.ThrowObjectNotFound<TEntity>();
        }

        return entity;
    }

    public static void UpdateOrThrow<TEntity>(this IRepository<TEntity> repository, object snowflake, Action<TEntity> action)
        => UpdateOrThrow(repository, snowflake, action, null!);

    public static void UpdateOrThrow<TEntity>(this IRepository<TEntity> repository, object snowflake, Action<TEntity> action, Action @default)
    {
        repository.Execute(repository =>
        {
            var affectedRows = repository.Update(snowflake, action);
            EnsureRowAffected<TEntity>(affectedRows, @default);
            EnsureSingleRowAffected<TEntity>(affectedRows, snowflake);
        });
    }

    public static object UpdateOrThrow<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> query, Action<TEntity> action)
    {
        var snowflakes = UpdateOrThrow(repository, query, 1, action, null!);
        var snowflake = snowflakes.Single();

        return snowflake;
    }

    public static object UpdateOrThrow<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> query, Action<TEntity> action, Action @default)
    {
        var snowflakes = UpdateOrThrow(repository, query, 1, action, @default);
        var snowflake = snowflakes.Single();

        return snowflake;
    }

    public static ICollection<object> UpdateOrThrow<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> query, int expectedRows, Action<TEntity> action)
        => UpdateOrThrow(repository,query, expectedRows, action, null!);

    public static ICollection<object> UpdateOrThrow<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> query, int expectedRows, Action<TEntity> action, Action @default)
    {
        ICollection<object> snowflakes = Array.Empty<object>();

        repository.Execute(repository =>
        {
            var affectedSnowflakes = repository.Update(query, action);
            var affectedRows = affectedSnowflakes.Count();
            EnsureRowAffected<TEntity>(affectedRows, @default);
            EnsureExpectedRowsAffected<TEntity>(affectedRows, expectedRows);

            snowflakes = affectedSnowflakes;
        });

        return snowflakes;
    }

    public static async Task UpdateOrThrowAsync<TEntity>(this IRepository<TEntity> repository, object snowflake, Action<TEntity> action)
        => await UpdateOrThrowAsync(repository, snowflake, action, null!);

    public static async Task UpdateOrThrowAsync<TEntity>(this IRepository<TEntity> repository, object snowflake, Action<TEntity> action, Action @default)
    {
        await repository.ExecuteAsync(async repository =>
        {
            var affectedRows = await repository.UpdateAsync(snowflake, action);
            EnsureRowAffected<TEntity>(affectedRows, @default);
            EnsureSingleRowAffected<TEntity>(affectedRows, snowflake);
        });
    }

    public static async Task<object> UpdateOrThrowAsync<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> query, Action<TEntity> action)
    {
        var snowflakes = await UpdateOrThrowAsync(repository, query, 1, action, null!);
        var snowflake = snowflakes.Single();

        return snowflake;
    }

    public static async Task<object> UpdateOrThrowAsync<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> query, Action<TEntity> action, Action fallback)
    {
        var snowflakes = await UpdateOrThrowAsync(repository, query, 1, action, fallback);
        var snowflake = snowflakes.Single();

        return snowflake;
    }

    public static async Task<ICollection<object>> UpdateOrThrowAsync<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> query, int expectedRows, Action<TEntity> action)
        => await UpdateOrThrowAsync(repository, query, expectedRows, action, null!);

    public static async Task<ICollection<object>> UpdateOrThrowAsync<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> query, int expectedRows, Action<TEntity> action, Action fallback)
    {
        ICollection<object> snowflakes = Array.Empty<object>();

        await repository.ExecuteAsync(async repository =>
        {
            var affectedSnowflakes = await repository.UpdateAsync(query, action);
            var affectedRows = affectedSnowflakes.Count();
            EnsureRowAffected<TEntity>(affectedRows, fallback);
            EnsureExpectedRowsAffected<TEntity>(affectedRows, expectedRows);

            snowflakes = affectedSnowflakes;
        });

        return snowflakes;
    }


    public static void DeleteOrThrow<TEntity>(this IRepository<TEntity> repository, object snowflake)
    {
        repository.Execute(repository =>
        {
            var affectedRows = repository.Delete(snowflake);
            EnsureSingleRowAffected<TEntity>(affectedRows, snowflake);
        });
    }

    public static object DeleteOrThrow<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> query)
    {
        var snowflakes = DeleteOrThrow(repository, query, 1);
        var snowflake = snowflakes.Single();

        return snowflake;
    }

    public static ICollection<object> DeleteOrThrow<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> query, int expectedRows)
    {
        ICollection<object> snowflakes = Array.Empty<object>();

        repository.Execute(repository =>
        {
            var affectedSnowflakes = repository.Delete(query);
            var affectedRows = affectedSnowflakes.Count();
            EnsureExpectedRowsAffected<TEntity>(affectedRows, expectedRows);

            snowflakes = affectedSnowflakes;
        });

        return snowflakes;
    }

    public static async Task DeleteOrThrowAsync<TEntity>(this IRepository<TEntity> repository, object snowflake)
    {
        await repository.ExecuteAsync(async repository =>
        {
            var affectedRows = await repository.DeleteAsync(snowflake);
            EnsureSingleRowAffected<TEntity>(affectedRows, snowflake);
        });
    }

    public static async Task<object> DeleteOrThrowAsync<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> query)
    {
        var snowflakes = await DeleteOrThrowAsync(repository, query, 1);
        var snowflake = snowflakes.Single();

        return snowflake;
    }

    public static async Task<ICollection<object>> DeleteOrThrowAsync<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> query, int expectedRows)
    {
        ICollection<object> snowflakes = Array.Empty<object>();

        await repository.ExecuteAsync(async repository =>
        {
            var affectedSnowflakes = await repository.DeleteAsync(query);
            var affectedRows = affectedSnowflakes.Count();
            EnsureExpectedRowsAffected<TEntity>(affectedRows, expectedRows);

            snowflakes = affectedSnowflakes;
        });

        return snowflakes;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void EnsureRowAffected<TEntity>(int affectedRows, Action? @default)
    {
        if (affectedRows == 0)
        {
            @default?.Invoke();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void EnsureSingleRowAffected<TEntity>(int affectedRows, object snowflake)
    {
        if (affectedRows == 0)
        {
            RepositoryException.ThrowObjectNotFound<TEntity>();
        }

        if (affectedRows != 1)
        {
            throw new UnreachableException($"Unexpected result of affected rows '{affectedRows}' after deleting {typeof(TEntity).Name} '{snowflake}'");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void EnsureExpectedRowsAffected<TEntity>(int affectedRows, int expectedRows)
    {
        if (affectedRows == 0 &&  expectedRows == 1)
        {
            RepositoryException.ThrowObjectNotFound<TEntity>();
        }

        if (affectedRows < expectedRows)
        {
            RepositoryException.ThrowObjectsNotFound<TEntity>(expectedRows, affectedRows);
        }

        if (affectedRows > expectedRows)
        {
            throw new UnreachableException($"Unexpected result of affected rows '{affectedRows}' after deleting '{expectedRows}' entities of type '{typeof(TEntity).Name}'");
        }
    }
}