using ChristianSchulz.MultitenancyMonolith.Shared.Authentication.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Authentication.Badge;

public class BadgeAuthenticationHandler : AuthenticationHandler<BadgeAuthenticationOptions>
{
    public BadgeAuthenticationHandler(IOptionsMonitor<BadgeAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock)
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

        var ticket = CreateTicket(badgeClaims, badgeIdentity);

        var context = new BadgeValidatePrincipalContext(Context, Scheme, Options, ticket);
        await Options.Events.ValidatePrincipal(context);

        if (context.Principal == null)
        {
            return AuthenticateResult.Fail("No principal.");
        }

        return AuthenticateResult.Success(new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name));
    }

    public static Claim[]? DeserializeBadge(string badge)
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

    public AuthenticationTicket CreateTicket(Claim[] badgeClaims, ClaimsIdentity identity)
    {
        var convertedClaims = new List<Claim>(badgeClaims);

        foreach (var action in Options.ClaimActions)
        {
            action.Run(convertedClaims, identity, ClaimsIssuer);
        }

        var principal = new ClaimsPrincipal(identity);

        return new AuthenticationTicket(principal, Scheme.Name);
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