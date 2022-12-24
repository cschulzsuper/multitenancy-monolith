using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace ChristianSchulz.MultitenancyMonolith.Data;

public static class RepositoryDeletionExtensions
{
    public static void DeleteOrThrow<TEntity>(this IRepository<TEntity> repository, object snowflake)
    {
        repository.Execute(repository => 
        {
            var rowsAffected = repository.Delete(snowflake);
            EnsureSingleAffectedRow<TEntity>(rowsAffected, snowflake);
        });
    }

    public static void DeleteOrThrow<TEntity>(this IRepository<TEntity> repository, TEntity entity)
    {
        repository.Execute(repository =>
        {
            var rowsAffected = repository.Delete(entity);
            EnsureSingleAffectedRow<TEntity>(rowsAffected);
        });
    }

    public static void DeleteOrThrow<TEntity>(this IRepository<TEntity> repository, params TEntity[] entities)
        => DeleteOrThrow(repository, entities as ICollection<TEntity>);

    public static void DeleteOrThrow<TEntity>(this IRepository<TEntity> repository, ICollection<TEntity> entities)
    {
        repository.Execute(repository =>
        {
            var relevantRows = entities.Count();
            var rowsAffected = repository.Delete(entities);
            EnsureRelevantRows<TEntity>(rowsAffected, relevantRows);
        });
    }

    public static void DeleteOrThrow<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> query)
        => DeleteOrThrow(repository, query, 1);

    public static void DeleteOrThrow<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> query, int relevantRows)
    {
        repository.Execute(repository =>
        {
            var rowsAffected = repository.Delete(query);
            EnsureRelevantRows<TEntity>(rowsAffected, relevantRows);
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

    public static async ValueTask DeleteOrThrowAsync<TEntity>(this IRepository<TEntity> repository, TEntity entity)
    {
        await repository.ExecuteAsync(async repository =>
        {
            var rowsAffected = await repository.DeleteAsync(entity);
            EnsureSingleAffectedRow<TEntity>(rowsAffected);
        });
    }

    public static async ValueTask DeleteOrThrowAsync<TEntity>(this IRepository<TEntity> repository, params TEntity[] entities)
        => await DeleteOrThrowAsync(repository, entities as ICollection<TEntity>);

    public static async ValueTask DeleteOrThrowAsync<TEntity>(this IRepository<TEntity> repository, ICollection<TEntity> entities)
    {
        await repository.ExecuteAsync(async repository =>
        {
            var relevantRows = entities.Count();
            var rowsAffected = await repository.DeleteAsync(entities);
            EnsureRelevantRows<TEntity>(rowsAffected, relevantRows);
        });
    }

    public static ValueTask DeleteOrThrowAsync<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> query)
        => DeleteOrThrowAsync(repository, query, 1);

    public static async ValueTask DeleteOrThrowAsync<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> query, int relevantRows)
    {
        await repository.ExecuteAsync(async repository =>
        {
            var rowsAffected = await repository.DeleteAsync(query);
            EnsureRelevantRows<TEntity>(rowsAffected, relevantRows);
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void EnsureSingleAffectedRow<TEntity>(int rowsAffected, object snowflake)
    {
        if (rowsAffected == 0)
        {
            throw new RepositoryException($"{typeof(TEntity).Name} '{snowflake}' could not be deleted");
        }

        if (rowsAffected != 1)
        {
            throw new UnreachableException($"Unexpected result of affected rows '{rowsAffected}' after deleting {typeof(TEntity).Name} '{snowflake}'");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void EnsureSingleAffectedRow<TEntity>(int rowsAffected)
    {
        if (rowsAffected == 0)
        {
            throw new RepositoryException($"Single entity of type '{typeof(TEntity).Name}' could not be deleted");
        }

        if (rowsAffected != 1)
        {
            throw new UnreachableException($"Unexpected result of affected rows '{rowsAffected}' after deleting single entity of type '{typeof(TEntity).Name}'");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void EnsureRelevantRows<TEntity>(int rowsAffected, int relevantRows)
    {
        if (rowsAffected < relevantRows)
        {
            throw new RepositoryException($"Entities of type '{typeof(TEntity).Name}' could not be deleted");
        }

        if (rowsAffected > relevantRows)
        {
            throw new UnreachableException($"Unexpected result of affected rows '{rowsAffected}' after deleting '{relevantRows}' entities of type '{typeof(TEntity).Name}'");
        }
    }
}
