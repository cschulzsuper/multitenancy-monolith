using ChristianSchulz.MultitenancyMonolith.Application.Administration;
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
    private IMemberManager? _memberManager;

    public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
    {
        _scheme = scheme;
        _context = context;
        _identityManager = context.RequestServices.GetRequiredService<IIdentityManager>();
        _memberManager = context.RequestServices.GetRequiredService<IMemberManager>();

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
        // decode badge
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

        // extract badge claims
        var badgeIdentity = badgeClaims.SingleOrDefault(x => x.Type == "Identity");
        var badgeGroup = badgeClaims.SingleOrDefault(x => x.Type == "Group");
        var badgeMember = badgeClaims.SingleOrDefault(x => x.Type == "Member");
        var badgeVerificationString = badgeClaims.SingleOrDefault(x => x.Type == "Verification");

        // try authenticate identity
        if (badgeIdentity == null || badgeVerificationString == null)
        {
            return false;
        }

        var badgeVerification = Convert.FromBase64String(badgeVerificationString.Value);

        var identity = _identityManager!.Get(badgeIdentity.Value);

        var badgeValid = badgeVerification.SequenceEqual(identity.Verification);
        if (badgeValid)
        {
            var claims = new[]
            {
                badgeIdentity,
                badgeVerificationString
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Badge");

            claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        }

        if (badgeGroup == null || badgeMember == null)
        {
            return claimsPrincipal != null;
        }

        // try authenticate group member
        var member = _memberManager!
            .GetAll(badgeGroup.Value)
            .SingleOrDefault(X => X.UniqueName == badgeMember.Value);

        if (member == null)
        {
            return false;
        }

        badgeValid = badgeVerification.SequenceEqual(member.Verification);
        if (badgeValid)
        {
            var claims = new[]
            {
                badgeIdentity,
                badgeGroup,
                badgeMember,
                badgeVerificationString,
                new Claim(ClaimTypes.Role, "Member")
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Badge");

            claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            return true;
        }

        return false;
    }
}