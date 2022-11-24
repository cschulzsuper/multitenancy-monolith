using System.Collections.Generic;
using System.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Core;

public abstract class ClaimAction
{
    public ClaimAction(string claimType, string valueType)
    {
        ClaimType = claimType;
        ValueType = valueType;
    }

    public string ClaimType { get; }

    public string ValueType { get; }

    public abstract void Run(ICollection<Claim> claims, ClaimsIdentity identity, string issuer);
}
