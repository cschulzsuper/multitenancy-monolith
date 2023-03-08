﻿using ChristianSchulz.MultitenancyMonolith.Shared.Metadata;
using System;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application;

public sealed class TransportException : Exception
{
    private TransportException(string message) : base(message) { }

    [DoesNotReturn]
    public static void ThrowNotFound<TEntity>(string uniqueName)
    {
        var objectType = ObjectAnnotations.ExtractObjectType<TEntity>();

        var exception = new TransportException($"Object '{objectType}' with unique name '{uniqueName}' not found.");

        exception.Data["error-code"] = "object-not-found";

        throw exception;
    }

    [DoesNotReturn]
    public static void ThrowSecurityViolation(string message)
    {
        var exception = new TransportException(message);

        exception.Data["error-code"] = "security";

        throw exception;
    }

}