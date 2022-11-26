using Microsoft.Extensions.Caching.Distributed;

namespace ChristianSchulz.MultitenancyMonolith.Caching;

public record ByteCache : IByteCache
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

        return _distributedCache.Get(cacheKey)
            ?? throw new CachingException($"Could not find cached entry for key '{cacheKey}'");
    }

    public void Set(string key, byte[] value)
    {
        var cacheKey = $"{_prefix}.{key}";

        _distributedCache.Set(cacheKey, value);
    }
}