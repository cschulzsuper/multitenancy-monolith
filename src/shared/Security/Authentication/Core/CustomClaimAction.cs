using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Core;

internal sealed class CustomClaimAction : ClaimAction
{
    public CustomClaimAction(string claimType, string valueType, Func<ICollection<Claim>, string?> resolver)
        : base(claimType, valueType)
    {
        Resolver = resolver;
    }

    public Func<ICollection<Claim>, string?> Resolver { get; }

    public override void Run(ICollection<Claim> claims, ClaimsIdentity identity, string issuer)
    {
        var value = Resolver(claims);
        if (!string.IsNullOrEmpty(value))
        {
            identity.AddClaim(new Claim(ClaimType, value, ValueType, issuer));
        }
    }
}