using ChristianSchulz.MultitenancyMonolith.Caching;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal sealed class MemberVerficationManager : IMemberVerficationManager
{
    private readonly IByteCache _byteCache;

    public MemberVerficationManager(IByteCacheFactory byteCacheFactory) 
    {
        _byteCache = byteCacheFactory.Create($"member-verfication");
    }

    public bool Has(string group, string member, byte[] verfication)
        => _byteCache.Has($"{group}.{member}", verfication);

    public void Set(string group, string member, byte[] verfication)
        => _byteCache.Set($"{group}.{member}", verfication);
}
