using ChristianSchulz.MultitenancyMonolith.Caching;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

internal sealed class AccountMemberVerificationManager : IAccountMemberVerificationManager
{
    private readonly IByteCache _byteCache;

    public AccountMemberVerificationManager(IByteCacheFactory byteCacheFactory)
    {
        _byteCache = byteCacheFactory.Create("account-member-verification");
    }

    public bool Has(AccountMemberVerificationKey verificationKey, byte[] verification)
        => _byteCache.Has($"{verificationKey.Identity}:{verificationKey.Group}:{verificationKey.Member}:{verificationKey.Client}", verification);

    public void Set(AccountMemberVerificationKey verificationKey, byte[] verification)
        => _byteCache.Set($"{verificationKey.Identity}:{verificationKey.Group}:{verificationKey.Member}:{verificationKey.Client}", verification);
}