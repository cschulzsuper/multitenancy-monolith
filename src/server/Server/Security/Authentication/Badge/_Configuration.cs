﻿using System;
using System.Linq;
using System.Security.Claims;
using ChristianSchulz.MultitenancyMonolith.Application.Authentication;
using ChristianSchulz.MultitenancyMonolith.Application.Administration;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using ChristianSchulz.MultitenancyMonolith.Shared.Authentication.Badge;
using ChristianSchulz.MultitenancyMonolith.Shared.Authentication.Core;

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

    public static bool Validate(BadgeValidatePrincipalContext context)
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
        var identityManager = context.HttpContext.RequestServices.GetRequiredService<IIdentityManager>();

        var badgeIdentity = badgeClaims.SingleOrDefault(x => x.Type == "Identity");
        if (badgeIdentity == null)
        {
            return false;
        }

        var identity = identityManager.Get(badgeIdentity.Value);

        var badgeValid = badgeVerification.SequenceEqual(identity.Verification);

        return badgeValid;
    }

    private static bool ValidateMember(BadgeValidatePrincipalContext context, ICollection<Claim> badgeClaims, byte[] badgeVerification)
    {
        var memberManager = context.HttpContext.RequestServices.GetRequiredService<IMemberManager>();

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

        var member = memberManager
            .GetAll(badgeGroup.Value)
            .SingleOrDefault(x => x.UniqueName == badgeMember.Value);

        if (member == null)
        {
            return false;
        }

        var badgeValid = badgeVerification.SequenceEqual(member.Verification);

        return badgeValid;
    }
}