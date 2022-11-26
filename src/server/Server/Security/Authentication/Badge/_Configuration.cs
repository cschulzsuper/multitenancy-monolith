using System;
using System.Linq;
using System.Security.Claims;
using ChristianSchulz.MultitenancyMonolith.Application.Authentication;
using ChristianSchulz.MultitenancyMonolith.Application.Administration;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Core;

namespace ChristianSchulz.MultitenancyMonolith.Server.Security.Authentication.Badge;

internal static class _Configuration
{
    public static void Configure(this BadgeAuthenticationOptions options)
    {
        options.ClaimActions.MapCustomClaim(ClaimTypes.Role, claims => claims.Any(x => x.Type == "Member") ? "Member" : null);

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

    private static bool Validate(BadgeValidatePrincipalContext context)
    {
        var badgeClaims = 
            context.Principal?.Claims as ICollection<Claim> ?? 
            context.Principal?.Claims.ToArray() ??
            Array.Empty<Claim>();

        var badgeVerificationString = badgeClaims.SingleOrDefault(x => x.Type == "Verification");
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

        var badgeIdentity = badgeClaims.SingleOrDefault(x => x.Type == "Identity");
        if (badgeIdentity == null)
        {
            return false;
        }

        var badgeValid = identityVerficationManager.Has(badgeIdentity.Value, badgeVerification);

        return badgeValid;
    }

    private static bool ValidateMember(BadgeValidatePrincipalContext context, ICollection<Claim> badgeClaims, byte[] badgeVerification)
    {
        var memberVerficationManager = context.HttpContext.RequestServices.GetRequiredService<IMemberVerficationManager>();

        var badgeGroup = badgeClaims.SingleOrDefault(x => x.Type == "Group");
        if (badgeGroup == null)
        {
            return false;
        }

        var badgeMember = badgeClaims.SingleOrDefault(x => x.Type == "Member");
        if (badgeMember == null)
        {
            return false;
        }

        var badgeValid = memberVerficationManager.Has(badgeGroup.Value, badgeMember.Value, badgeVerification);

        return badgeValid;
    }
}