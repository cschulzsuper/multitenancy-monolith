using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Authentication.Badge;
public class BadgeValidatePrincipalContext : PrincipalContext<BadgeAuthenticationOptions>
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
