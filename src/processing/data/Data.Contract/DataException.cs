using ChristianSchulz.MultitenancyMonolith.Shared.Metadata;
using System;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Data;

public sealed class DataException : Exception
{
    private DataException(string message) : base(message) { }

    private DataException(string message, Exception innerException) : base(message, innerException) { }

    [DoesNotReturn]
    public static void ThrowObjectConflict<TEntity>(object snowflake)
    {
        var objectType = ObjectAnnotations.ExtractObjectType<TEntity>();

        var exception = new DataException($"Object '{objectType}' with snowflake '{snowflake}' already exists.");

        exception.Data["error-code"] = "object-conflict";

        throw exception;
    }

    [DoesNotReturn]
    public static void ThrowObjectConflict<TEntity>(string messageParameter)
    {
        var objectType = ObjectAnnotations.ExtractObjectType<TEntity>();

        var exception = new DataException($"Object '{objectType}' causes conflict '{messageParameter}'.");

        exception.Data["error-code"] = "object-conflict";

        throw exception;
    }

    [DoesNotReturn]
    public static void ThrowObjectConflict(Exception innerException)
    {
        var exception = new DataException($"Object already existst.", innerException);

        exception.Data["error-code"] = "object-conflict";

        throw exception;
    }

    [DoesNotReturn]
    public static void ThrowObjectNotFound<TEntity>()
    {
        var objectType = ObjectAnnotations.ExtractObjectType<TEntity>();

        var exception = new DataException($"Object '{objectType}' not found.");

        exception.Data["error-code"] = "object-not-found";

        throw exception;
    }

    [DoesNotReturn]
    public static void ThrowObjectNotFound<TEntity>(object snowflake)
    {
        var objectType = ObjectAnnotations.ExtractObjectType<TEntity>();

        var exception = new DataException($"Object '{objectType}' with snowflake '{snowflake}' not found.");

        exception.Data["error-code"] = "object-not-found";

        throw exception;
    }

    [DoesNotReturn]
    public static void ThrowObjectsNotFound<TEntity>(int expected, int actual)
    {
        var objectType = ObjectAnnotations.ExtractObjectType<TEntity>();

        var exception = new DataException($"Objects '{objectType}' not found. Expect '{expected}' but found '{actual}'.");

        exception.Data["error-code"] = "object-not-found";

        throw exception;
    }

    [DoesNotReturn]
    public static void ThrowTransactionFailed()
    {
        var exception = new DataException($"Unable to commit transaction.");

        exception.Data["error-code"] = "transaction-failed";

        throw exception;
    }

    [DoesNotReturn]
    public static void ThrowUniqueNameConflict<TEntity>(string uniqueName)
    {
        var entityType = typeof(TEntity);

        ThrowUniqueNameConflict(entityType, uniqueName);
    }

    [DoesNotReturn]
    public static void ThrowUniqueNameConflict(Type entityType, string uniqueName)
    {
        var objectType = ObjectAnnotations.ExtractObjectType(entityType);

        var exception = new DataException($"Unique name '{uniqueName}' of object '{objectType}' causes conflict.");

        exception.Data["error-code"] = "object-conflict";

        throw exception;
    }

    [DoesNotReturn]
    public static void ThrowPropertyValueConflict<TEntity>(string propertyName, object propertyValue)
    {
        var objectType = ObjectAnnotations.ExtractObjectType<TEntity>();

        var exception = new DataException($"Property '{propertyName}' value '{propertyValue}' of object '{objectType}' causes conflict.");

        exception.Data["error-code"] = "object-conflict";

        throw exception;
    }

    [DoesNotReturn]
    public static void ThrowPropertyNameConflict<TEntity>(string propertyName)
    {
        var objectType = ObjectAnnotations.ExtractObjectType<TEntity>();

        var exception = new DataException($"Property name '{propertyName}' of object '{objectType}' causes conflict.");

        exception.Data["error-code"] = "object-conflict";

        throw exception;
    }

    [DoesNotReturn]
    public static void ThrowPropertyTypeMismatch<TEntity>(string propertyName)
    {
        var objectType = ObjectAnnotations.ExtractObjectType<TEntity>();

        var exception = new DataException($"Object '{objectType}' property '{propertyName}' has incorrect type.");

        exception.Data["error-code"] = "object-invalid";

        throw exception;
    }
}