using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge.Mapping;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;


namespace ChristianSchulz.MultitenancyMonolith.Server.Ticker.Security;

internal static class BadgeConfiguration
{
    public static void Configure(this BadgeAuthenticationOptions options, ICollection<AllowedClient> allowedClients)
    {
        options.ClaimActions.MapRoleClaimForAdmin();

        options.ClaimActions.MapRoleClaimForChief();
        options.ClaimActions.MapRoleClaimForMember();

        options.ClaimActions.MapRoleClaimForTicker();

        foreach (var allowedClient in allowedClients)
        {
            foreach (var scope in allowedClient.Scopes)
            {
                options.ClaimActions.MapScopeClaim(allowedClient.UniqueName, scope);
            }
        }

        options.Events.OnValidatePrincipal = async context =>
        {
            var validator = context.HttpContext.RequestServices
                .GetService<BadgeValidator>() ?? new BadgeValidator();

            var valid = await validator.ValidateAsync(context);

            if (!valid)
            {
                context.RejectPrincipal();
            }
        };
    }

    private static void MapRoleClaimForAdmin(this ICollection<ClaimAction> claimActions)
        => claimActions.MapCustomClaim(ClaimTypes.Role, claims =>
            claims.Any(x => x.Type == "identity" && x.Value == "admin")

            ? "admin" : null);

    private static void MapRoleClaimForChief(this ICollection<ClaimAction> claimActions)
        => claimActions.MapCustomClaim(ClaimTypes.Role, claims =>
            claims.Any(x => x.Type == "badge" && x.Value == "member") &&
            claims.Any(x => x.Type == "member" && x.Value.StartsWith("chief-"))

            ? "chief" : null);

    private static void MapRoleClaimForMember(this ICollection<ClaimAction> claimActions)
        => claimActions.MapCustomClaim(ClaimTypes.Role, claims =>
            claims.Any(x => x.Type == "badge" && x.Value == "member") &&
            claims.Any(x => x.Type == "member" && !string.IsNullOrWhiteSpace(x.Value))

            ? "member" : null);

    private static void MapRoleClaimForTicker(this ICollection<ClaimAction> claimActions)
        => claimActions.MapCustomClaim(ClaimTypes.Role, claims =>
            claims.Any(x => x.Type == "badge" && x.Value == "ticker") &&
            claims.Any(x => x.Type == "mail" && !string.IsNullOrWhiteSpace(x.Value))

            ? "ticker" : null);

    private static void MapScopeClaim(this ICollection<ClaimAction> claimActions, string client, string scope)
        => claimActions.MapCustomClaim("scope", claims =>
            claims.Any(x => x.Type == "client" && x.Value == client)

            ? scope : null);
}