using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge;

public sealed class BadgeValidatePrincipalContext : PrincipalContext<BadgeAuthenticationOptions>
{
    public BadgeValidatePrincipalContext(HttpContext context, AuthenticationScheme scheme, BadgeAuthenticationOptions options, AuthenticationTicket ticket)
        : base(context, scheme, options, ticket?.Properties)
    {
        ArgumentNullException.ThrowIfNull(ticket, nameof(ticket));

        Principal = ticket.Principal;
    }

    public void ReplacePrincipal(ClaimsPrincipal principal) => Principal = principal;

    public void RejectPrincipal() => Principal = null;
}
