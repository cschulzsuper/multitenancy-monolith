using System;
using System.Diagnostics;
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

    public static async ValueTask<TEntity> GetAsync<TEntity>(this IRepository<TEntity> repository, object snowflake)
    {
        var entity = await repository.GetOrDefaultAsync(snowflake);

        if (entity == null)
        {
            RepositoryException.ThrowObjectNotFound<TEntity>(snowflake);
        }

        return entity;
    }

    public static async ValueTask<TEntity> GetAsync<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate)
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
            var rowsAffected = repository.Update(snowflake, action);
            DefaultSingleAffectedRow<TEntity>(rowsAffected, @default);
            EnsureSingleAffectedRow<TEntity>(rowsAffected, snowflake);
        });
    }

    public static void UpdateOrThrow<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> query, Action<TEntity> action)
        => UpdateOrThrow(repository, query, 1, action, null!);

    public static void UpdateOrThrow<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> query, Action<TEntity> action, Action @default)
        => UpdateOrThrow(repository, query, 1, action, @default);

    public static void UpdateOrThrow<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> query, int expectedRows, Action<TEntity> action)
        => UpdateOrThrow(repository,query, expectedRows, action, null!);

    public static void UpdateOrThrow<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> query, int expectedRows, Action<TEntity> action, Action @default)
    {
        repository.Execute(repository =>
        {
            var rowsAffected = repository.Update(query, action);
            DefaultSingleAffectedRow<TEntity>(rowsAffected, @default);
            EnsureRelevantRows<TEntity>(rowsAffected, expectedRows);
        });
    }

    public static async ValueTask UpdateOrThrowAsync<TEntity>(this IRepository<TEntity> repository, object snowflake, Action<TEntity> action)
        => await UpdateOrThrowAsync(repository, snowflake, action, null!);

    public static async ValueTask UpdateOrThrowAsync<TEntity>(this IRepository<TEntity> repository, object snowflake, Action<TEntity> action, Action @default)
    {
        await repository.ExecuteAsync(async repository =>
        {
            var rowsAffected = await repository.UpdateAsync(snowflake, action);
            DefaultSingleAffectedRow<TEntity>(rowsAffected, @default);
            EnsureSingleAffectedRow<TEntity>(rowsAffected, snowflake);
        });
    }

    public static async ValueTask UpdateOrThrowAsync<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> query, Action<TEntity> action)
        => await UpdateOrThrowAsync(repository, query, 1, action, null!);
    
    public static async ValueTask UpdateOrThrowAsync<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> query, Action<TEntity> action, Action @default)
        => await UpdateOrThrowAsync(repository, query, 1, action, @default);

    public static async ValueTask UpdateOrThrowAsync<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> query, int expectedRows, Action<TEntity> action)
        => await UpdateOrThrowAsync(repository, query, expectedRows, action, null!);

    public static async ValueTask UpdateOrThrowAsync<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> query, int expectedRows, Action<TEntity> action, Action @default)
    {
        await repository.ExecuteAsync(async repository =>
        {
            var rowsAffected = await repository.UpdateAsync(query, action);
            DefaultSingleAffectedRow<TEntity>(rowsAffected, @default);
            EnsureRelevantRows<TEntity>(rowsAffected, expectedRows);
        });
    }


    public static void DeleteOrThrow<TEntity>(this IRepository<TEntity> repository, object snowflake)
    {
        repository.Execute(repository =>
        {
            var rowsAffected = repository.Delete(snowflake);
            EnsureSingleAffectedRow<TEntity>(rowsAffected, snowflake);
        });
    }

    public static void DeleteOrThrow<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> query)
        => DeleteOrThrow(repository, query, 1);

    public static void DeleteOrThrow<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> query, int expectedRows)
    {
        repository.Execute(repository =>
        {
            var rowsAffected = repository.Delete(query);
            EnsureRelevantRows<TEntity>(rowsAffected, expectedRows);
        });
    }

    public static async ValueTask DeleteOrThrowAsync<TEntity>(this IRepository<TEntity> repository, object snowflake)
    {
        await repository.ExecuteAsync(async repository =>
        {
            var rowsAffected = await repository.DeleteAsync(snowflake);
            EnsureSingleAffectedRow<TEntity>(rowsAffected, snowflake);
        });
    }

    public static async ValueTask DeleteOrThrowAsync<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> query)
        => await DeleteOrThrowAsync(repository, query, 1);

    public static async ValueTask DeleteOrThrowAsync<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> query, int expectedRows)
    {
        await repository.ExecuteAsync(async repository =>
        {
            var rowsAffected = await repository.DeleteAsync(query);
            EnsureRelevantRows<TEntity>(rowsAffected, expectedRows);
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void DefaultSingleAffectedRow<TEntity>(int rowsAffected, Action? @default)
    {
        if (rowsAffected == 0)
        {
            @default?.Invoke();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void EnsureSingleAffectedRow<TEntity>(int rowsAffected, object snowflake)
    {
        if (rowsAffected == 0)
        {
            RepositoryException.ThrowObjectNotFound<TEntity>();
        }

        if (rowsAffected != 1)
        {
            throw new UnreachableException($"Unexpected result of affected rows '{rowsAffected}' after deleting {typeof(TEntity).Name} '{snowflake}'");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void EnsureRelevantRows<TEntity>(int rowsAffected, int expectedRows)
    {
        if (rowsAffected == 0 &&  expectedRows == 1)
        {
            RepositoryException.ThrowObjectNotFound<TEntity>();
        }

        if (rowsAffected < expectedRows)
        {
            RepositoryException.ThrowObjectsNotFound<TEntity>(expectedRows, rowsAffected);
        }

        if (rowsAffected > expectedRows)
        {
            throw new UnreachableException($"Unexpected result of affected rows '{rowsAffected}' after deleting '{expectedRows}' entities of type '{typeof(TEntity).Name}'");
        }
    }
}