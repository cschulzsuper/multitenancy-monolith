using ChristianSchulz.MultitenancyMonolith.Caching;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

internal sealed class AuthenticationIdentityVerificationManager : IAuthenticationIdentityVerificationManager
{
    private readonly IByteCache _byteCache;

    public AuthenticationIdentityVerificationManager(IByteCacheFactory byteCacheFactory)
    {
        _byteCache = byteCacheFactory.Create("authentication-identity-verification");
    }

    public bool Has(AuthenticationIdentityVerificationKey verificationKey, byte[] verification)
        => _byteCache.Has($"{verificationKey.AuthenticationIdentity}:{verificationKey.ClientName}", verification);

    public void Set(AuthenticationIdentityVerificationKey verificationKey, byte[] verification)
        => _byteCache.Set($"{verificationKey.AuthenticationIdentity}:{verificationKey.ClientName}", verification);
}