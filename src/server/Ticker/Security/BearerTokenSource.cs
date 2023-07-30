using Microsoft.AspNetCore.Http;

namespace ChristianSchulz.MultitenancyMonolith.Server.Security;

public static class BearerTokenSource
{
    public static string? GetTokenFromHeaders(HttpContext context)
    {
        var authorizationHeader = context.Request.Headers.Authorization.ToString();

        return !string.IsNullOrWhiteSpace(authorizationHeader)
            ? authorizationHeader.Replace("Bearer ", string.Empty)
            : null;
    }

    public static string? GetTokenFromCookies(HttpContext context)
    {
        var hasAuthorizationCookie = context.Request.Cookies
            .TryGetValue("access_token", out var authorizationCookie);

        return hasAuthorizationCookie && !string.IsNullOrWhiteSpace(authorizationCookie)
            ? authorizationCookie
            : null;
    }

    public static string? GetTokenFromQuery(HttpContext context)
    {
        var hasAuthorizationQuery = context.Request.Query
            .TryGetValue("access_token", out var authorizationQuery);

        return hasAuthorizationQuery && !string.IsNullOrWhiteSpace(authorizationQuery)
            ? authorizationQuery.ToString()
            : null;
    }
}