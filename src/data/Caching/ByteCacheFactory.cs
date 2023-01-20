using Microsoft.Extensions.Caching.Distributed;

namespace ChristianSchulz.MultitenancyMonolith.Caching;

internal sealed class ByteCacheFactory : IByteCacheFactory
{
    private readonly IDistributedCache _distributedCache;

    public ByteCacheFactory(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public IByteCache Create(string type)
        => new ByteCache(_distributedCache, type);
}