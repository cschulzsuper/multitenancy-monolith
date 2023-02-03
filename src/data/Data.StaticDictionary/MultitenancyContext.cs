using System.Security.Claims;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary;

internal sealed class MultitenancyContext
{
    public MultitenancyContext(ClaimsPrincipal user)
    {
        MultitenancyDiscriminator = user.GetClaimOrDefault("group") ?? string.Empty;
    }

    public string MultitenancyDiscriminator { get; set; }
}