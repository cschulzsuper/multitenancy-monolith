using ChristianSchulz.MultitenancyMonolith.Backend.Server;
using ChristianSchulz.MultitenancyMonolith.Shared.Multitenancy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]

internal static class MockWebApplication
{
    public const string ClientName = "endpoint-tests";
    public const string AuthenticationIdentity = "admin";
    public const string AccountGroup = "group";
    public const string AccountMember = "chief-member";

    private static readonly IDictionary<string, string> _configuration = new Dictionary<string, string>()
    {
        {"AllowedClients:0:Service", "endpoint-tests"},
        {"AllowedClients:0:Scopes:1", "endpoints"},
        {"AllowedClients:0:Scopes:2", "openapi-json"},
    };

    public static WebApplicationFactory<Program> Create()
    {
        var application = new WebApplicationFactory<Program>();

        application = application
            .WithWebHostBuilder(app => app
            .ConfigureServices(services =>
            {
                services.Configure<BearerTokenOptions>(BearerTokenDefaults.AuthenticationScheme, options =>
                {
                    options.Events.OnMessageReceived = context => Task.CompletedTask;
                });
            })
            .ConfigureAppConfiguration((_, config) =>
            {
                config.Sources.Clear();
                config.AddInMemoryCollection(_configuration!);
            }));

        return application;
    }

    public static IServiceScope CreateMultitenancyScope(this WebApplicationFactory<Program> factory)
        => factory.Services.CreateMultitenancyScope(AccountGroup);

    private static string ProtectClaims(this WebApplicationFactory<Program> factory, Claim[] claims)
    {
        var options = factory.Services.GetRequiredService<IOptionsMonitor<BearerTokenOptions>>().Get(BearerTokenDefaults.AuthenticationScheme);

        var claimsIdentity = new ClaimsIdentity(claims, BearerTokenDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var authenticationProperties = new AuthenticationProperties
        {
            ExpiresUtc = DateTime.UtcNow + options.BearerTokenExpiration
        };

        var authenticationTicket = new AuthenticationTicket(claimsPrincipal, authenticationProperties, $"{BearerTokenDefaults.AuthenticationScheme}:AccessToken");

        var token = options.BearerTokenProtector.Protect(authenticationTicket);

        return token;
    }

    public static AuthenticationHeaderValue MockValidIdentityAuthorizationHeader(this WebApplicationFactory<Program> factory)
    {
        var claims = new Claim[]
        {
            new ("type", "identity"),
            new ("client-name", ClientName),
            new ("authentication-identity", AuthenticationIdentity)
        };

        var token = factory.ProtectClaims(claims);

        return new AuthenticationHeaderValue("Bearer", token);

    }

    public static AuthenticationHeaderValue MockValidMemberAuthorizationHeader(this WebApplicationFactory<Program> factory)
    {
        var claims = new Claim[]
        {
            new ("type", "member"),
            new ("client-name", ClientName),
            new ("authentication-identity", AuthenticationIdentity),
            new ("account-group", AccountGroup),
            new ("account-member", AccountMember)
        };

        var token = factory.ProtectClaims(claims);

        return new AuthenticationHeaderValue("Bearer", token);
    }
}