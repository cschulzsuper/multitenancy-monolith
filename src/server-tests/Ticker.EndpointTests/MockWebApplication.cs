using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using ChristianSchulz.MultitenancyMonolith.Shared.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]
internal static class MockWebApplication
{
    public const string ClientName = "endpoint-tests";

    public const string AuthenticationIdentity = "admin";
    public const string AccountGroup = "group";
    public const string AccountMember = "default";
    public const string Mail = "default@localhost";

    private static readonly IDictionary<string, string> _configuration = new Dictionary<string, string>()
    {
        {"ServiceMappings:0:UniqueName", ClientName},
        {"ServiceMappings:0:Url", "http://localhost"},

        {"AdmissionServer:Service", ClientName},

        {"AllowedClients:0:Service", ClientName},
        {"AllowedClients:0:Scopes:0", "swagger-json"},
        {"AllowedClients:0:Scopes:1", "endpoints"},
    };

    public static WebApplicationFactory<Program> Mock(this WebApplicationFactory<Program> factory, ITestOutputHelper? output = null)
        => factory.WithWebHostBuilder(app => app
            .ConfigureServices(services =>
            {
                services.Configure<BearerTokenOptions>(BearerTokenDefaults.AuthenticationScheme, options =>
                {
                    options.Events.OnMessageReceived = context => Task.CompletedTask;
                });
            })
            .ConfigureLogging(loggingBuilder =>
            {
                if (output != null)
                {
                    loggingBuilder.Services.AddSingleton<ILoggerProvider>(_ => new XunitLoggerProvider(output));
                }
            })
            .ConfigureAppConfiguration((_, config) =>
            {
                config.Sources.Clear();
                config.AddInMemoryCollection(_configuration!);
            }));


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

    public static AuthenticationHeaderValue MockValidMemberAuthorizationHeader(this WebApplicationFactory<Program> factory)
    {
        var claims = new Claim[]
        {
            new Claim("type", "member"),
            new Claim("client", ClientName),
            new Claim("identity", AuthenticationIdentity),
            new Claim("group", AccountGroup),
            new Claim("member", AccountMember)
        };

        var token = factory.ProtectClaims(claims);

        return new AuthenticationHeaderValue("Bearer", token);
    }

    public static AuthenticationHeaderValue MockValidIdentityAuthorizationHeader(this WebApplicationFactory<Program> factory)
    {
        var claims = new Claim[]
        {
            new Claim("type", "identity"),
            new Claim("client", ClientName),
            new Claim("identity", AuthenticationIdentity)
        };

        var token = factory.ProtectClaims(claims);

        return new AuthenticationHeaderValue("Bearer", token);
    }

    public static AuthenticationHeaderValue MockValidTickerAuthorizationHeader(this WebApplicationFactory<Program> factory)
    {
        var claims = new Claim[]
        {
            new Claim("type", "ticker"),
            new Claim("client", ClientName),
            new Claim("group", AccountGroup),
            new Claim("mail", Mail)
        };

        var token = factory.ProtectClaims(claims);

        return new AuthenticationHeaderValue("Bearer", token);
    }
}