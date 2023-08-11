using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary;
using ChristianSchulz.MultitenancyMonolith.Server;
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
    public const string Client = "endpoint-tests";

    public const string AuthenticationIdentity = "admin";
    public const string MailAddress = "default@localhost";
    public const string Secret = "default";

    public const string Group = "group";
    public const string Member = "chief-member";

    private static readonly IDictionary<string, string> _configuration = new Dictionary<string, string>()
    {
        {"AllowedClients:0:Service", "swagger"},
        {"AllowedClients:0:Scopes:0", "swagger-json"},
        {"AllowedClients:0:Scopes:1", "endpoints"},
        {"AllowedClients:1:Service", "endpoint-tests"},
        {"AllowedClients:1:Scopes:1", "endpoints"},
        {"AllowedClients:2:Service", "security-tests"},
        {"AllowedClients:2:Scopes:1", "endpoints"}
    };

    public static WebApplicationFactory<Program> Mock(this WebApplicationFactory<Program> factory)
    => factory.WithWebHostBuilder(app => app
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

    public static IServiceScope CreateMultitenancyScope(this WebApplicationFactory<Program> factory)
        => factory.Services.CreateMultitenancyScope(Group);

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
            new Claim("type", "identity"),
            new Claim("client", Client),
            new Claim("identity", AuthenticationIdentity)
        };

        var token = factory.ProtectClaims(claims);

        return new AuthenticationHeaderValue("Bearer", token);

    }

    public static AuthenticationHeaderValue MockValidMemberAuthorizationHeader(this WebApplicationFactory<Program> factory)
    {
        var claims = new Claim[]
        {
            new Claim("type", "member"),
            new Claim("client", Client),
            new Claim("identity", AuthenticationIdentity),
            new Claim("group", Group),
            new Claim("member", Member)
        };

        var token = factory.ProtectClaims(claims);

        return new AuthenticationHeaderValue("Bearer", token);
    }
}