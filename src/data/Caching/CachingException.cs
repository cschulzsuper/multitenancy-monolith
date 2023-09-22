using System;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Caching;

public sealed class CachingException : Exception
{
    private CachingException(string message) : base(message) { }

    [DoesNotReturn]
    public static void ThrowCacheKeyNotFound(string cacheKey)
    {
        var exception = new CachingException($"Could not find cached entry for key '{cacheKey}'");

        exception.Data["error-code"] = "cache-key-not-found";

        throw exception;
    }
}