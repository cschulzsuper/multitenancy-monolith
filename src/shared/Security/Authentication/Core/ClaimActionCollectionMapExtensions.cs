using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Core;

public static class ClaimActionCollectionMapExtensions
{
    public static void MapCustomClaim(this ICollection<ClaimAction> collection, string claimType, Func<ICollection<Claim>, string?> resolver)
    {
        ArgumentNullException.ThrowIfNull(collection, nameof(collection));

        collection.MapCustomClaim(claimType, ClaimValueTypes.String, resolver);
    }

    public static void MapCustomClaim(this ICollection<ClaimAction> collection, string claimType, string valueType, Func<ICollection<Claim>, string?> resolver)
    {
        ArgumentNullException.ThrowIfNull(collection, nameof(collection));

        collection.Add(new CustomClaimAction(claimType, valueType, resolver));
    }
}