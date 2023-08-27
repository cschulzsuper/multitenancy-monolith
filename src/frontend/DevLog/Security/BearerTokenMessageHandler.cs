using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Frontend.DevLog.Security;

public static class BearerTokenMessageHandler
{
    public static async Task Handle<TBearerTokenValidator>(MessageReceivedContext context)
        where TBearerTokenValidator : BearerTokenValidator
    {
        context.Token =
            GetTokenFromHeaders(context.HttpContext) ??
            GetTokenFromCookies(context.HttpContext) ??
            GetTokenFromQuery(context.HttpContext);

        var ticket = context.Options.BearerTokenProtector.Unprotect(context.Token);
        if (ticket == null)
        {
            context.Fail("Unprotected token failed");
            return;
        }

        await Activator.CreateInstance<TBearerTokenValidator>()
            .ValidateAsync(context, ticket);
    }

    private static string? GetTokenFromHeaders(HttpContext context)
    {
        var authorizationHeader = context.Request.Headers.Authorization.ToString();

        return !string.IsNullOrWhiteSpace(authorizationHeader)
            ? authorizationHeader.Replace("Bearer ", string.Empty)
            : null;
    }

    private static string? GetTokenFromCookies(HttpContext context)
    {
        var hasAuthorizationCookie = context.Request.Cookies
            .TryGetValue("access_token", out var authorizationCookie);

        return hasAuthorizationCookie && !string.IsNullOrWhiteSpace(authorizationCookie)
            ? authorizationCookie
            : null;
    }

    private static string? GetTokenFromQuery(HttpContext context)
    {
        var hasAuthorizationQuery = context.Request.Query
            .TryGetValue("access_token", out var authorizationQuery);

        return hasAuthorizationQuery && !string.IsNullOrWhiteSpace(authorizationQuery)
            ? authorizationQuery.ToString()
            : null;
    }
}