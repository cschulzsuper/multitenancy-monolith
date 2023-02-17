using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge.Mapping;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Server.Security;

internal static class BadgeConfiguration
{
    public static void Configure(this BadgeAuthenticationOptions options)
    {
        options.ClaimActions.MapRoleClaimForAdmin();
        options.ClaimActions.MapRoleClaimForIdentity();
        options.ClaimActions.MapRoleClaimForDemo();

        options.ClaimActions.MapRoleClaimForChief();
        options.ClaimActions.MapRoleClaimForChiefObserver();
        options.ClaimActions.MapRoleClaimForMember();
        options.ClaimActions.MapRoleClaimForMemberObserver();

        options.ClaimActions.MapScopeClaimForEndpoints();
        options.ClaimActions.MapScopeClaimForSwaggerJson();

        options.Events.OnValidatePrincipal = context =>
        {
            var validator = context.HttpContext.RequestServices
                .GetService<BadgeValidator>() ?? new BadgeValidator();

            var valid = validator.Validate(context);
            if (!valid)
            {
                context.RejectPrincipal();
            }

            return Task.CompletedTask;
        };
    }

    private static void MapRoleClaimForAdmin(this ICollection<ClaimAction> claimActions)
        => claimActions.MapCustomClaim(ClaimTypes.Role, claims =>
            claims.Any(x => x.Type == "identity" && x.Value == "admin")

            ? "admin" : null);

    private static void MapRoleClaimForIdentity(this ICollection<ClaimAction> claimActions)
        => claimActions.MapCustomClaim(ClaimTypes.Role, claims =>
            claims.Any(x => x.Type == "identity" && x.Value != "demo")

            ? "identity" : null);

    private static void MapRoleClaimForDemo(this ICollection<ClaimAction> claimActions)
        => claimActions.MapCustomClaim(ClaimTypes.Role, claims => 
            claims.Any(x => x.Type == "identity" && x.Value == "demo")

            ? "default" : null);

    private static void MapRoleClaimForChief(this ICollection<ClaimAction> claimActions)
        => claimActions.MapCustomClaim(ClaimTypes.Role, claims =>
            claims.Any(x => x.Type == "badge" && x.Value == "member") &&
            claims.Any(x => x.Type == "identity" && x.Value != "demo") &&
            claims.Any(x => x.Type == "member" && x.Value.StartsWith("chief-"))

            ? "chief" : null);

    private static void MapRoleClaimForChiefObserver(this ICollection<ClaimAction> claimActions)
        => claimActions.MapCustomClaim(ClaimTypes.Role, claims =>
            claims.Any(x => x.Type == "badge" && x.Value == "member") &&
            claims.Any(x => x.Type == "identity" && x.Value == "demo") &&
            claims.Any(x => x.Type == "member" && x.Value.StartsWith("chief-"))

            ? "chief-observer" : null);

    private static void MapRoleClaimForMember(this ICollection<ClaimAction> claimActions)
        => claimActions.MapCustomClaim(ClaimTypes.Role, claims =>
            claims.Any(x => x.Type == "badge" && x.Value == "member") &&
            claims.Any(x => x.Type == "identity" && x.Value != "demo") &&
            claims.Any(x => x.Type == "member" && !string.IsNullOrWhiteSpace(x.Value))

            ? "member" : null);

    private static void MapRoleClaimForMemberObserver(this ICollection<ClaimAction> claimActions)
    => claimActions.MapCustomClaim(ClaimTypes.Role, claims =>
        claims.Any(x => x.Type == "badge" && x.Value == "member") &&
        claims.Any(x => x.Type == "identity" && x.Value == "demo") &&
        claims.Any(x => x.Type == "member" && !string.IsNullOrWhiteSpace(x.Value))

        ? "member-observer" : null);

    private static void MapScopeClaimForEndpoints(this ICollection<ClaimAction> claimActions)
        => claimActions.MapCustomClaim("scope", claims =>
            claims.Any(x => x.Type == "client" && x.Value == "endpoint-tests") ||
            claims.Any(x => x.Type == "client" && x.Value == "security-tests")

            ? "endpoints" : null);

    private static void MapScopeClaimForSwaggerJson(this ICollection<ClaimAction> claimActions)
        => claimActions.MapCustomClaim("scope", claims =>
            claims.Any(x => x.Type == "client" && x.Value == "swagger")

            ? "swagger-json" : null);
}