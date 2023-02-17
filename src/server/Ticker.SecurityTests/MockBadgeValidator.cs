﻿using ChristianSchulz.MultitenancyMonolith.Server.Ticker.Security;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

internal sealed class MockBadgeValidator : BadgeValidator
{
    protected override Task<bool> ValidateMemberAsync(BadgeValidatePrincipalContext context)
    {
        var badgeClaims =
            context.Principal?.Claims as ICollection<Claim> ??
            context.Principal?.Claims.ToArray() ??
            Array.Empty<Claim>();

        var badgeMember = badgeClaims.SingleOrDefault(x => x.Type == "member");
        if (badgeMember == null)
        {
            return Task.FromResult(false);
        }

        if (badgeMember.Value != MockWebApplication.Member)
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }
}