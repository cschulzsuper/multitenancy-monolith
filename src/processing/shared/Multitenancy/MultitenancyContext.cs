using ChristianSchulz.MultitenancyMonolith.Shared.Security.Claims;
using System.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Multitenancy;

public sealed class MultitenancyContext
{
    public MultitenancyContext(ClaimsPrincipal user)
    {
        MultitenancyDiscriminator = user.GetClaimOrDefault("account-group") ?? string.Empty;
    }

    public string MultitenancyDiscriminator { get; internal set; }
}