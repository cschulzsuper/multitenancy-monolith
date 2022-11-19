using ChristianSchulz.MultitenancyMonolith.Application.Authentication;
using ChristianSchulz.MultitenancyMonolith.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Server.BadgeIdentity;

public class BadgeAuthenticationHandler : IAuthenticationHandler
{
    private AuthenticationScheme? _scheme;
    private HttpContext? _context;
    private IIdentityManager? _identityManager;

    public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
    {
        _scheme = scheme;
        _context = context;
        _identityManager = context.RequestServices.GetRequiredService<IIdentityManager>();

        return Task.CompletedTask;
    }

    public Task<AuthenticateResult> AuthenticateAsync()
    {
        var contextBadge =
            GetBadgeFromHeaders() ??
            GetBadgeFromCookies() ??
            GetBadgeFromQuery();

        if (contextBadge == null)
        {
            return Task.FromResult(AuthenticateResult.Fail("Authentication failed"));
        }

        var authenticated = TryAuthenticate(contextBadge, out var claimsPrincipal);
        if (!authenticated)
        {
            return Task.FromResult(AuthenticateResult.Fail("Authentication failed"));
        }

        var ticket = new AuthenticationTicket(claimsPrincipal!, _scheme!.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    private string? GetBadgeFromHeaders()
    {
        var authorizationHeader = _context!.Request.Headers.Authorization.ToString();

        return !string.IsNullOrWhiteSpace(authorizationHeader)
            ? authorizationHeader.Replace("Bearer ", string.Empty)
            : null;
    }

    private string? GetBadgeFromCookies()
    {
        var hasAuthorizationCookie = _context!.Request.Cookies
            .TryGetValue("access_token", out var authorizationCookie);

        return hasAuthorizationCookie && !string.IsNullOrWhiteSpace(authorizationCookie)
            ? authorizationCookie
            : null;
    }

    private string? GetBadgeFromQuery()
    {
        var hasAuthorizationQuery = _context!.Request.Query
            .TryGetValue("access_token", out var authorizationQuery);

        return hasAuthorizationQuery && !string.IsNullOrWhiteSpace(authorizationQuery)
            ? authorizationQuery.ToString()
            : null;
    }

    public Task ChallengeAsync(AuthenticationProperties? properties)
    {
        _context!.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    }

    public Task ForbidAsync(AuthenticationProperties? properties)
    {
        _context!.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    }

    private bool TryAuthenticate(string badge, [MaybeNullWhen(false)] out ClaimsPrincipal claimsPrincipal)
    {
        claimsPrincipal = null;

        if (string.IsNullOrWhiteSpace(badge))
        {
            return false;
        }

        var badgeBytes = WebEncoders.Base64UrlDecode(badge);
        var badgeClaims = JsonSerializer.Deserialize<Claim[]>(badgeBytes, ClaimsJsonSerializerOptions.Options)
             ?? Array.Empty<Claim>();

        if (!badgeClaims.Any())
        {
            return false;
        }

        var identityUniqueName = badgeClaims.SingleOrDefault(x => x.Type == "Identity")?.Value;        
        var identityVerificationString = badgeClaims.SingleOrDefault(x => x.Type == "Verification")?.Value;

        if (identityUniqueName == null || identityVerificationString == null)
        {
            return false;
        }

        var identityVerification = Convert.FromBase64String(identityVerificationString);

        var identity = _identityManager!.Get(identityUniqueName);

        var badgeValid = identityVerification.SequenceEqual(identity.Verification);
        if (!badgeValid)
        {
            return false;
        }

        var claims = Array.Empty<Claim>();
        var claimsIdentity = new ClaimsIdentity(claims, "Badge");
        
        claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        return true;
    }
}