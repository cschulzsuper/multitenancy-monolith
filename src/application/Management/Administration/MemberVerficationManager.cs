using ChristianSchulz.MultitenancyMonolith.Caching;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal sealed class MemberVerficationManager : IMemberVerficationManager
{
    private readonly IByteCache _byteCache;

    public MemberVerficationManager(IByteCacheFactory byteCacheFactory) 
    {
        _byteCache = byteCacheFactory.Create($"member-verfication");
    }

    public byte[] Get(string member)
        => _byteCache.Get(member);

    public void Set(string member, byte[] verfication)
        => _byteCache.Set(member,verfication);
}
