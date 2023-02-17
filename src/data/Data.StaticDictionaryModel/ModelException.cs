using ChristianSchulz.MultitenancyMonolith.Shared.Metadata;
using System;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel;

public sealed class ModelException : Exception
{
    private ModelException(string message) : base(message) { }

    [DoesNotReturn]
    public static void ThrowPropertyTypeMismatch<TEntity>(string property)
    {
        var objectType = ObjectAnnotations.ExtractObjectType<TEntity>();

        var exception = new ModelException($"Object '{objectType}' property '{property}' has incorrect type.");

        exception.Data["error-code"] = "object-invalid";

        throw exception;
    }

    [DoesNotReturn]
    public static void ThrowObjectConflict<TEntity>(string messageParameter)
    {
        var objectType = ObjectAnnotations.ExtractObjectType<TEntity>();

        var exception = new ModelException($"Object '{objectType}' causes conflict '{messageParameter}'.");

        exception.Data["error-code"] = "object-conflict";

        throw exception;
    }

    [DoesNotReturn]
    public static void ThrowUniqueNameConflict<TEntity>(string uniqueName)
    {
        var objectType = ObjectAnnotations.ExtractObjectType<TEntity>();

        var exception = new ModelException($"Unique name '{uniqueName}' of object '{objectType}' causes conflict.");

        exception.Data["error-code"] = "object-conflict";

        throw exception;
    }

    [DoesNotReturn]
    public static void ThrowPropertyNameConflict<TEntity>(string propertyName)
    {
        var objectType = ObjectAnnotations.ExtractObjectType<TEntity>();

        var exception = new ModelException($"Property name '{propertyName}' of object '{objectType}' causes conflict.");

        exception.Data["error-code"] = "object-conflict";

        throw exception;
    }

    [DoesNotReturn]
    public static void ThrowMailAddressConflict<TEntity>(string mailAddress)
    {
        var objectType = ObjectAnnotations.ExtractObjectType<TEntity>();

        var exception = new ModelException($"Mail address '{mailAddress}' of object '{objectType}' causes conflict.");

        exception.Data["error-code"] = "object-conflict";

        throw exception;
    }
}