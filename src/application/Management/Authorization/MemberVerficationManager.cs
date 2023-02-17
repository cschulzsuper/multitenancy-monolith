using ChristianSchulz.MultitenancyMonolith.Caching;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authorization;

internal sealed class MemberVerificationManager : IMemberVerificationManager
{
    private readonly IByteCache _byteCache;

    public MemberVerificationManager(IByteCacheFactory byteCacheFactory)
    {
        _byteCache = byteCacheFactory.Create("membership-verification");
    }

    public bool Has(MemberVerificationKey verificationKey, byte[] verification)
        => _byteCache.Has($"{verificationKey.Identity}:{verificationKey.Group}:{verificationKey.Member}:{verificationKey.Client}", verification);

    public void Set(MemberVerificationKey verificationKey, byte[] verification)
        => _byteCache.Set($"{verificationKey.Identity}:{verificationKey.Group}:{verificationKey.Member}:{verificationKey.Client}", verification);
}