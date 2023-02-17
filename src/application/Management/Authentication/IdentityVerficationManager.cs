using ChristianSchulz.MultitenancyMonolith.Caching;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

internal sealed class IdentityVerificationManager : IIdentityVerificationManager
{
    private readonly IByteCache _byteCache;

    public IdentityVerificationManager(IByteCacheFactory byteCacheFactory)
    {
        _byteCache = byteCacheFactory.Create("identity-verification");
    }

    public bool Has(IdentityVerificationKey verificationKey, byte[] verification)
        => _byteCache.Has($"{verificationKey.Identity}:{verificationKey.Client}", verification);

    public void Set(IdentityVerificationKey verificationKey, byte[] verification)
        => _byteCache.Set($"{verificationKey.Identity}:{verificationKey.Client}", verification);
}