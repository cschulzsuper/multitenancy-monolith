using ChristianSchulz.MultitenancyMonolith.Caching;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

internal sealed class AuthenticationIdentityVerificationManager : IAuthenticationIdentityVerificationManager
{
    private readonly IByteCache _byteCache;

    public AuthenticationIdentityVerificationManager(IByteCacheFactory byteCacheFactory)
    {
        _byteCache = byteCacheFactory.Create("authentication-identity-verification");
    }

    public bool Has(IdentityVerificationKey verificationKey, byte[] verification)
        => _byteCache.Has($"{verificationKey.Identity}:{verificationKey.Client}", verification);

    public void Set(IdentityVerificationKey verificationKey, byte[] verification)
        => _byteCache.Set($"{verificationKey.Identity}:{verificationKey.Client}", verification);
}