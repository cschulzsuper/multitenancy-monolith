using ChristianSchulz.MultitenancyMonolith.Server.Ticker.Security;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge;
using System.Threading.Tasks;

internal sealed class MockBadgeValidator : BadgeValidator
{
    public override Task<bool> ValidateAsync(BadgeValidatePrincipalContext _)
        => Task.FromResult(true);
}
