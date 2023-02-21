using ChristianSchulz.MultitenancyMonolith.Server.Security;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge;

internal sealed class MockBadgeValidator : BadgeValidator
{
    public override bool Validate(BadgeValidatePrincipalContext context)
        => true;
}
