using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge;

public sealed class BadgeAuthenticationHandler : AuthenticationHandler<BadgeAuthenticationOptions>
{
    public BadgeAuthenticationHandler(IOptionsMonitor<BadgeAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder)
        : base(options, logger, encoder)
    { }

    protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var contextBadge =
            GetBadgeFromHeaders() ??
            GetBadgeFromCookies() ??
            GetBadgeFromQuery();

        if (contextBadge == null)
        {
            return AuthenticateResult.Fail("No badge.");
        }

        var badgeClaims = DeserializeBadge(contextBadge);
        if (badgeClaims == null)
        {
            return AuthenticateResult.Fail("Badge deserialization failed.");
        }

        var badgeIdentity = new ClaimsIdentity(badgeClaims, "Badge");

        var ticket = new AuthenticationTicket(new ClaimsPrincipal(badgeIdentity), Scheme.Name);

        var context = new BadgeValidatePrincipalContext(Context, Scheme, Options, ticket);

        await Options.Events.ValidatePrincipal(context);

        if (context.Principal == null)
        {
            return AuthenticateResult.Fail("No principal.");
        }

        return AuthenticateResult.Success(new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name));
    }

    private static Claim[]? DeserializeBadge(string badge)
    {
        if (string.IsNullOrWhiteSpace(badge))
        {
            return null;
        }

        var badgeBytes = WebEncoders.Base64UrlDecode(badge);
        var badgeClaims = JsonSerializer.Deserialize<Claim[]>(badgeBytes, ClaimsJsonSerializerOptions.Options)
             ?? null;

        return badgeClaims;
    }

    private string? GetBadgeFromHeaders()
    {
        var authorizationHeader = Context.Request.Headers.Authorization.ToString();

        return !string.IsNullOrWhiteSpace(authorizationHeader)
            ? authorizationHeader.Replace("Bearer ", string.Empty)
            : null;
    }

    private string? GetBadgeFromCookies()
    {
        var hasAuthorizationCookie = Context.Request.Cookies
            .TryGetValue("access_token", out var authorizationCookie);

        return hasAuthorizationCookie && !string.IsNullOrWhiteSpace(authorizationCookie)
            ? authorizationCookie
            : null;
    }

    private string? GetBadgeFromQuery()
    {
        var hasAuthorizationQuery = Context.Request.Query
            .TryGetValue("access_token", out var authorizationQuery);

        return hasAuthorizationQuery && !string.IsNullOrWhiteSpace(authorizationQuery)
            ? authorizationQuery.ToString()
            : null;
    }
}