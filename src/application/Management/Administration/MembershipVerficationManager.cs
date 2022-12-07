using ChristianSchulz.MultitenancyMonolith.Caching;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal sealed class MembershipVerficationManager : IMembershipVerficationManager
{
    private readonly IByteCache _byteCache;

    public MembershipVerficationManager(IByteCacheFactory byteCacheFactory) 
    {
        _byteCache = byteCacheFactory.Create($"membership-verfication");
    }

    public bool Has(string group, string member, byte[] verfication)
        => _byteCache.Has($"{group}.{member}", verfication);

    public void Set(string group, string member, byte[] verfication)
        => _byteCache.Set($"{group}.{member}", verfication);
}
