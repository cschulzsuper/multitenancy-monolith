using ChristianSchulz.MultitenancyMonolith.Application.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Server.BadgeIdentity;

public class BadgeAuthenticationHandler : IAuthenticationHandler
{
    private AuthenticationScheme? _scheme;
    private HttpContext? _context;
    private IIdentityManager? _identityManager;

    private static readonly int SizeOfGuid = Marshal.SizeOf(typeof(Guid));

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

        var badgeBytes = Convert
            .FromBase64String(badge)
            .AsSpan();

        if (badgeBytes.Length <= SizeOfGuid)
        {
            return false;
        }

        var verfication = badgeBytes.Slice(0, SizeOfGuid);
        var identityBytes = badgeBytes.Slice(SizeOfGuid);
        var identity = Encoding.UTF8.GetString(identityBytes);

        var user = _identityManager!.Get(identity);

        var badgeValid = verfication.SequenceEqual(user.Verification);
        if (!badgeValid)
        {
            return false;
        }

        var claims = Array.Empty<Claim>();
        var claimsIdentity = new ClaimsIdentity(claims, "badge");
        
        claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        return true;
    }
}