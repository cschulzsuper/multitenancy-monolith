using ChristianSchulz.MultitenancyMonolith.Application.Administration;
using ChristianSchulz.MultitenancyMonolith.Application.Authentication;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Server.Security.Authentication.Badge;

internal static class _Configuration
{
    public static void Configure(this BadgeAuthenticationOptions options)
    {
        options.ClaimActions.MapRoleClaimForAdmin();
        options.ClaimActions.MapRoleClaimForDefault();
        options.ClaimActions.MapRoleClaimForSecure();

        options.ClaimActions.MapRoleClaimForChief();
        options.ClaimActions.MapRoleClaimForMember();
        options.ClaimActions.MapRoleClaimForObserver();

        options.ClaimActions.MapScopeClaimForEndpoints();
        options.ClaimActions.MapScopeClaimForSwaggerJson();

        options.Events.OnValidatePrincipal = context =>
        {
            var valid = Validate(context);

            if (!valid)
            {
                context.RejectPrincipal();
            }

            return Task.CompletedTask;
        };
    }

    private static void MapRoleClaimForAdmin(this ICollection<ClaimAction> claimActions)
        => claimActions.MapCustomClaim(ClaimTypes.Role,
            claims => claims.Any(x =>
                x.Type == "identity" && x.Value == "admin")

            ? "admin" : null);

    private static void MapRoleClaimForDefault(this ICollection<ClaimAction> claimActions)
        => claimActions.MapCustomClaim(ClaimTypes.Role,
            claims => claims.Any(x =>
                x.Type == "identity" &&
                !string.IsNullOrWhiteSpace(x.Value))

            ? "default" : null);

    private static void MapRoleClaimForSecure(this ICollection<ClaimAction> claimActions)
        => claimActions.MapCustomClaim(ClaimTypes.Role,
            claims => claims.Any(x =>
                x.Type == "identity" &&
                x.Value != "demo")

            ? "secure" : null);

    private static void MapRoleClaimForChief(this ICollection<ClaimAction> claimActions)
        => claimActions.MapCustomClaim(ClaimTypes.Role,
            claims => claims.Any(x =>
                x.Type == "member" &&
                x.Value.StartsWith("chief-"))

            ? "chief" : null);

    private static void MapRoleClaimForMember(this ICollection<ClaimAction> claimActions)
        => claimActions.MapCustomClaim(ClaimTypes.Role,
            claims => claims.Any(x =>
                x.Type == "member" &&
                !string.IsNullOrWhiteSpace(x.Value))

            ? "member" : null);

    private static void MapRoleClaimForObserver(this ICollection<ClaimAction> claimActions)
        => claimActions.MapCustomClaim(ClaimTypes.Role,
            claims =>
                claims.Any(x =>
                    x.Type == "identity" &&
                    x.Value == "demo") &&
                claims.Any(x =>
                    x.Type == "member" &&
                    !string.IsNullOrWhiteSpace(x.Value))

            ? "observer" : null);

    private static void MapScopeClaimForEndpoints(this ICollection<ClaimAction> claimActions)
        => claimActions.MapCustomClaim("scope",
            claims => claims.Any(x =>
                x.Type == "client" &&
                (x.Value == "endpoint-tests"))

            ? "endpoints" : null);

    private static void MapScopeClaimForSwaggerJson(this ICollection<ClaimAction> claimActions)
        => claimActions.MapCustomClaim("scope",
            claims => claims.Any(x =>
                x.Type == "client" &&
                x.Value == "swagger")

            ? "swagger-json" : null);

    private static bool Validate(BadgeValidatePrincipalContext context)
    {
        var badgeClaims =
            context.Principal?.Claims as ICollection<Claim> ??
            context.Principal?.Claims.ToArray() ??
            Array.Empty<Claim>();

        var badgeVerificationString = badgeClaims.SingleOrDefault(x => x.Type == "verification");
        if (badgeVerificationString == null)
        {
            return false;
        }

        var badgeVerification = Convert.FromBase64String(badgeVerificationString.Value);

        return
            ValidateIdentity(context, badgeClaims, badgeVerification) ||
            ValidateMember(context, badgeClaims, badgeVerification);
    }

    private static bool ValidateIdentity(BadgeValidatePrincipalContext context, ICollection<Claim> badgeClaims, byte[] badgeVerification)
    {
        var identityVerficationManager = context.HttpContext.RequestServices.GetRequiredService<IIdentityVerficationManager>();

        var badgeClient = badgeClaims.SingleOrDefault(x => x.Type == "client");
        if (badgeClient == null)
        {
            return false;
        }

        var badgeIdentity = badgeClaims.SingleOrDefault(x => x.Type == "identity");
        if (badgeIdentity == null)
        {
            return false;
        }

        var verficationKey = new IdentityVerficationKey
        {
            Client = badgeClient.Value,
            Identity = badgeIdentity.Value,
        };

        var badgeValid = identityVerficationManager.Has(verficationKey, badgeVerification);

        return badgeValid;
    }

    private static bool ValidateMember(BadgeValidatePrincipalContext context, ICollection<Claim> badgeClaims, byte[] badgeVerification)
    {
        var membershipVerficationManager = context.HttpContext.RequestServices.GetRequiredService<IMembershipVerficationManager>();

        var badgeClient = badgeClaims.SingleOrDefault(x => x.Type == "client");
        if (badgeClient == null)
        {
            return false;
        }

        var badgeGroup = badgeClaims.SingleOrDefault(x => x.Type == "group");
        if (badgeGroup == null)
        {
            return false;
        }

        var badgeMember = badgeClaims.SingleOrDefault(x => x.Type == "member");
        if (badgeMember == null)
        {
            return false;
        }

        var verficationKey = new MembershipVerficationKey
        {
            Client = badgeClient.Value,
            Group = badgeGroup.Value,
            Member = badgeMember.Value,
        };

        var badgeValid = membershipVerficationManager.Has(verficationKey, badgeVerification);

        return badgeValid;
    }
}