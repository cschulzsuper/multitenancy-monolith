using Microsoft.Extensions.Caching.Distributed;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Caching;

internal sealed class ByteCache : IByteCache
{
    private readonly string _prefix;
    private readonly IDistributedCache _distributedCache;

    public ByteCache(IDistributedCache distributedCache, string prefix)
    {
        _prefix = prefix;
        _distributedCache = distributedCache;
    }

    public byte[] Get(string key)
    {
        var cacheKey = $"{_prefix}.{key}";
        var cacheValue = _distributedCache.Get(cacheKey);

        if (cacheValue == null)
        {
            CachingException.ThrowCacheKeyNotFound(cacheKey);
        }

        return cacheValue;
    }

    public bool Has(string key, byte[] value)
    {
        var cacheKey = $"{_prefix}.{key}";

        var cacheValue = _distributedCache.Get(cacheKey);

        return
            cacheValue != null &&
            cacheValue.SequenceEqual(value);
    }

    public void Set(string key, byte[] value)
    {
        var cacheKey = $"{_prefix}.{key}";

        _distributedCache.Set(cacheKey, value);
    }
}