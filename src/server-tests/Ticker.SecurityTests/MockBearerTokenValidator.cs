using ChristianSchulz.MultitenancyMonolith.Server.Ticker.Security;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

internal sealed class MockBearerTokenValidator : BearerTokenValidator
{
    protected override Task ValidateMemberAsync(MessageReceivedContext context, AuthenticationTicket ticket)
    {
        var claims =
            ticket.Principal?.Claims as ICollection<Claim> ??
            ticket.Principal?.Claims.ToArray() ??
            Array.Empty<Claim>();

        var groupClaim = claims.SingleOrDefault(x => x.Type == "group");
        if (groupClaim == null)
        {
            context.Fail("Token has no group");
            return Task.CompletedTask;
        }

        if (groupClaim.Value != MockWebApplication.AccountGroup)
        {
            context.Fail("Token has invalid verification");
            return Task.CompletedTask;
        }

        var memberClaim = claims.SingleOrDefault(x => x.Type == "member");
        if (memberClaim == null)
        {
            context.Fail("Token has no member");
            return Task.CompletedTask;
        }

        if (memberClaim.Value != MockWebApplication.AccountGroupMember &&
            memberClaim.Value != MockWebApplication.AccountGroupChief)
        {
            context.Fail("Token has invalid verification");
            return Task.CompletedTask;
        }

        context.Principal = ticket.Principal;
        context.Success();
        return Task.CompletedTask;
    }

    protected override Task ValidateIdentityAsync(MessageReceivedContext context, AuthenticationTicket ticket)
    {
        var claims =
            ticket.Principal?.Claims as ICollection<Claim> ??
            ticket.Principal?.Claims.ToArray() ??
            Array.Empty<Claim>();

        var identityClaim = claims.SingleOrDefault(x => x.Type == "identity");
        if (identityClaim == null)
        {
            context.Fail("Token has no identity");
            return Task.CompletedTask;
        }

        if (identityClaim.Value != MockWebApplication.AuthenticationIdentityAdmin &&
            identityClaim.Value != MockWebApplication.AuthenticationIdentityIdentity &&
            identityClaim.Value != MockWebApplication.AuthenticationIdentityDemo)
        {
            context.Fail("Token has invalid verification");
            return Task.CompletedTask;
        }

        context.Principal = ticket.Principal;
        context.Success();
        return Task.CompletedTask;
    }
}
