using ChristianSchulz.MultitenancyMonolith.Caching;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

internal sealed class IdentityVerficationManager : IIdentityVerficationManager
{
    private readonly IByteCache _byteCache;

    public IdentityVerficationManager(IByteCacheFactory byteCacheFactory)
    {
        _byteCache = byteCacheFactory.Create("identity-verfication");
    }

    public bool Has(IdentityVerficationKey verficationKey, byte[] verfication)
        => _byteCache.Has($"{verficationKey.Identity}:{verficationKey.Client}", verfication);

    public void Set(IdentityVerficationKey verficationKey, byte[] verfication)
        => _byteCache.Set($"{verficationKey.Identity}:{verficationKey.Client}", verfication);
}