using ChristianSchulz.MultitenancyMonolith.Caching;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

internal sealed class TickerUserVerificationManager : ITickerUserVerificationManager
{
    private readonly IByteCache _byteCache;

    public TickerUserVerificationManager(IByteCacheFactory byteCacheFactory)
    {
        _byteCache = byteCacheFactory.Create("ticker-user-verification");
    }

    public bool Has(TickerUserVerificationKey verificationKey, byte[] verification)
        => _byteCache.Has($"{verificationKey.Group}:{verificationKey.Mail}:{verificationKey.Client}", verification);

    public void Set(TickerUserVerificationKey verificationKey, byte[] verification)
        => _byteCache.Set($"{verificationKey.Group}:{verificationKey.Mail}:{verificationKey.Client}", verification);
}