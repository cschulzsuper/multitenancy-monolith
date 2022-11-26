using ChristianSchulz.MultitenancyMonolith.Caching;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

internal sealed class IdentityVerficationManager : IIdentityVerficationManager
{
    private readonly IByteCache _byteCache;

    public IdentityVerficationManager(IByteCacheFactory byteCacheFactory)
    {
        _byteCache = byteCacheFactory.Create("identity-verfication");
    }

    public bool Has(string identity, byte[] verfication)
        => _byteCache.Has(identity, verfication);

    public void Set(string identity, byte[] verfication)
        => _byteCache.Set(identity, verfication);
}
