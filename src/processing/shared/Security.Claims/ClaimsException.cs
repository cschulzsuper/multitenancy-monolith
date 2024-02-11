using System;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Security.Claims;

public sealed class ClaimsException : Exception
{
    private ClaimsException(string message) : base(message) { }

    [DoesNotReturn]
    public static void ThrowClaimNotFound(string claim)
    {
        var exception = new ClaimsException($"Could not find '{claim}' claim.");

        exception.Data["error-code"] = "claim-not-found";

        throw exception;
    }
}