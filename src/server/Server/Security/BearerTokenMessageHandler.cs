using ChristianSchulz.MultitenancyMonolith.Shared.Security.Claims;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Server.Security;

public static class BearerTokenMessageHandler
{
    private static readonly string[] allowedAuthenticationTypes = ["identity", "member"];

    public static Task Handle(MessageReceivedContext context)
    {
        context.Token =
            GetTokenFromHeaders(context.HttpContext) ??
            GetTokenFromQuery(context.HttpContext);

        var ticket = context.Options.BearerTokenProtector.Unprotect(context.Token);
        if (ticket == null)
        {
            context.Fail("Unprotected token failed");
            return Task.CompletedTask;
        }

        var authenticationType = ticket.Principal.GetClaimOrDefault("type");
        var authenticationTypeAllowed = allowedAuthenticationTypes.Contains(authenticationType);

        if (!authenticationTypeAllowed)
        {
            context.Fail($"Authentication type '{authenticationType}' not allowed");
            return Task.CompletedTask;
        }

        context.Principal = ticket.Principal;
        context.Success();
        return Task.CompletedTask;
    }

    private static string? GetTokenFromHeaders(HttpContext context)
    {
        var authorizationHeader = context.Request.Headers.Authorization.ToString();

        return !string.IsNullOrWhiteSpace(authorizationHeader)
            ? authorizationHeader.Replace("Bearer ", string.Empty)
            : null;
    }

    private static string? GetTokenFromQuery(HttpContext context)
    {
        var hasAuthorizationQuery = context.Request.Query
            .TryGetValue("access-token", out var authorizationQuery);

        return hasAuthorizationQuery && !string.IsNullOrWhiteSpace(authorizationQuery)
            ? authorizationQuery.ToString()
            : null;
    }
}