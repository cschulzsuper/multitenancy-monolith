using ChristianSchulz.MultitenancyMonolith.Caching;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal sealed class MembershipVerficationManager : IMembershipVerficationManager
{
    private readonly IByteCache _byteCache;

    public MembershipVerficationManager(IByteCacheFactory byteCacheFactory)
    {
        _byteCache = byteCacheFactory.Create($"membership-verfication");
    }

    public bool Has(MembershipVerficationKey verficationKey, byte[] verfication)
        => _byteCache.Has($"{verficationKey.Group}:{verficationKey.Member}:{verficationKey.Client}", verfication);

    public void Set(MembershipVerficationKey verficationKey, byte[] verfication)
        => _byteCache.Set($"{verficationKey.Group}:{verficationKey.Member}:{verficationKey.Client}", verfication);
}