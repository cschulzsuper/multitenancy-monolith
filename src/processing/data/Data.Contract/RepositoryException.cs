using ChristianSchulz.MultitenancyMonolith.Shared.Metadata;
using System;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Data;

public sealed class RepositoryException : Exception
{
    private RepositoryException(string message) : base(message) { }

    private RepositoryException(string message, Exception innerException) : base(message, innerException) { }

    [DoesNotReturn]
    public static void ThrowObjectNotFound<TEntity>()
    {
        var objectType = ObjectAnnotations.ExtractObjectType<TEntity>();

        var exception = new RepositoryException($"Object '{objectType}' not found.");

        exception.Data["error-code"] = "object-not-found";

        throw exception;
    }

    [DoesNotReturn]
    public static void ThrowObjectNotFound<TEntity>(object snowflake)
    {
        var objectType = ObjectAnnotations.ExtractObjectType<TEntity>();

        var exception = new RepositoryException($"Object '{objectType}' with snowflake '{snowflake}' not found.");

        exception.Data["error-code"] = "object-not-found";

        throw exception;
    }

    [DoesNotReturn]
    public static void ThrowObjectsNotFound<TEntity>(int expected, int actual)
    {
        var objectType = ObjectAnnotations.ExtractObjectType<TEntity>();

        var exception = new RepositoryException($"Objects '{objectType}' not found. Expect '{expected}' but found '{actual}'.");

        exception.Data["error-code"] = "object-not-found";

        throw exception;
    }

    [DoesNotReturn]
    public static void ThrowObjectConflict<TEntity>(object snowflake)
    {
        var objectType = ObjectAnnotations.ExtractObjectType<TEntity>();

        var exception = new RepositoryException($"Object '{objectType}' with snowflake '{snowflake}' already existst.");

        exception.Data["error-code"] = "object-conflict";

        throw exception;
    }

    [DoesNotReturn]
    public static void ThrowObjectConflict(Exception innerException)
    {
        var exception = new RepositoryException($"Object already existst.", innerException);

        exception.Data["error-code"] = "object-conflict";

        throw exception;
    }

    public static void ThrowTransactionFailed()
    {
        var exception = new RepositoryException($"Unable to commit transaction.");

        exception.Data["error-code"] = "transaction-failed";

        throw exception;
    }
}