using ChristianSchulz.MultitenancyMonolith.Caching;
using System.Security.Cryptography;
using System.Text;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

internal sealed class AuthenticationIdentityVerificationManager : IAuthenticationIdentityVerificationManager
{
    private readonly IByteCache _byteCache;

    public AuthenticationIdentityVerificationManager(IByteCacheFactory byteCacheFactory)
    {
        _byteCache = byteCacheFactory.Create("authentication-identity-verification");
    }

    public bool Has(AuthenticationIdentityVerificationKey verificationKey, byte[] verification)
    {
        var key = CalculateKey(verificationKey);
        return _byteCache.Has(key, verification);
    }

    public void Set(AuthenticationIdentityVerificationKey verificationKey, byte[] verification)
    {
        var key = CalculateKey(verificationKey);
        _byteCache.Set(key, verification);
    }

    private static string CalculateKey(AuthenticationIdentityVerificationKey verificationKey)
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.Append(verificationKey.ClientName);
        stringBuilder.Append(':');
        stringBuilder.Append(verificationKey.AuthenticationIdentity);

        var input = stringBuilder.ToString();

        var inputBytes = Encoding.UTF8.GetBytes(input);
        var inputHash = MD5.HashData(inputBytes);

        return Convert.ToBase64String(inputHash);
    }
}