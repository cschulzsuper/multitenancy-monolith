using ChristianSchulz.MultitenancyMonolith.Caching;
using System;
using System.Security.Cryptography;
using System.Text;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

internal sealed class TickerUserVerificationManager : ITickerUserVerificationManager
{
    private readonly IByteCache _byteCache;

    public TickerUserVerificationManager(IByteCacheFactory byteCacheFactory)
    {
        _byteCache = byteCacheFactory.Create("ticker-user-verification");
    }

    public bool Has(TickerUserVerificationKey verificationKey, byte[] verification)
    {
        var key = CalculateKey(verificationKey);
        return _byteCache.Has(key, verification);
    }

    public void Set(TickerUserVerificationKey verificationKey, byte[] verification)
    {
        var key = CalculateKey(verificationKey);
        _byteCache.Set(key, verification);
    }

    private string CalculateKey(TickerUserVerificationKey verificationKey)
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.Append(verificationKey.Client);
        stringBuilder.Append(':');
        stringBuilder.Append(verificationKey.Group);
        stringBuilder.Append(':');
        stringBuilder.Append(verificationKey.Mail);

        var input = stringBuilder.ToString();

        using MD5 md5 = MD5.Create();

        var inputBytes = Encoding.UTF8.GetBytes(input);
        var inputHash = md5.ComputeHash(inputBytes);

        return Convert.ToBase64String(inputHash);
    }
}