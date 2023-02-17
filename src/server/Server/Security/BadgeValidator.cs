﻿using ChristianSchulz.MultitenancyMonolith.Application.Authentication;
using ChristianSchulz.MultitenancyMonolith.Application.Authorization;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Server.Security;

public class BadgeValidator
{
    public virtual bool Validate(BadgeValidatePrincipalContext context)
    {
        var badgeClaims =
            context.Principal?.Claims as ICollection<Claim> ??
            context.Principal?.Claims.ToArray() ??
            Array.Empty<Claim>();

        var badgeType = badgeClaims.SingleOrDefault(x => x.Type == "badge");
        if (badgeType == null)
        {
            return false;
        }

        var badgeValid = badgeType.Value switch
        {
            "identity" => ValidateIdentity(context, badgeClaims),
            "member" => ValidateMember(context, badgeClaims),

            _ => false
        };

        return badgeValid;
    }

    private static bool ValidateIdentity(BadgeValidatePrincipalContext context, ICollection<Claim> badgeClaims)
    {
        var identityVerificationManager = context.HttpContext.RequestServices.GetRequiredService<IIdentityVerificationManager>();

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

        var verificationKey = new IdentityVerificationKey
        {
            Client = badgeClient.Value,
            Identity = badgeIdentity.Value,
        };

        var badgeVerification = badgeClaims.SingleOrDefault(x => x.Type == "verification");
        if (badgeVerification == null)
        {
            return false;
        }

        var badgeVerificationValue = Convert.FromBase64String(badgeVerification.Value);

        var badgeValid = identityVerificationManager.Has(verificationKey, badgeVerificationValue);

        return badgeValid;
    }

    private static bool ValidateMember(BadgeValidatePrincipalContext context, ICollection<Claim> badgeClaims)
    {
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

        var verificationKey = new MemberVerificationKey
        {
            Client = badgeClient.Value,
            Identity = badgeIdentity.Value,
            Group = badgeGroup.Value,
            Member = badgeMember.Value,
        };

        var badgeVerification = badgeClaims.SingleOrDefault(x => x.Type == "verification");
        if (badgeVerification == null)
        {
            return false;
        }

        var badgeVerificationValue = Convert.FromBase64String(badgeVerification.Value);

        var badgeValid = context.HttpContext.RequestServices
            .GetRequiredService<IMemberVerificationManager>()
            .Has(verificationKey, badgeVerificationValue);

        return badgeValid;
    }
}
