using ChristianSchulz.MultitenancyMonolith.Shared.Security.Claims;
using System.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Data;

internal sealed class MultitenancyContext
{
    public MultitenancyContext(ClaimsPrincipal user)
    {
        MultitenancyDiscriminator = user.GetClaimOrDefault("Group") ?? string.Empty;
    }   

    public string MultitenancyDiscriminator { get; set; }
}
