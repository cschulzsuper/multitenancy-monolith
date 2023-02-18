using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker.Security;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge.Serialization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

internal static class MockWebApplication
{
    public const int MockMember = 1;
    public const int MockTicker = 2;

    public const string Client = "multitenancy-tests";

    public const string Group1 = "group-1";
    public const string Group2 = "group-2";
    public const string Member = "default";
    public const string Mail = "default@localhost";

    private static readonly IDictionary<string, string> _configuration = new Dictionary<string, string>()
    {
        {"AllowedClients:0:UniqueName", "multitenancy-tests"},
        {"AllowedClients:0:Scopes:1", "endpoints"}
    };

    public static WebApplicationFactory<Program> Mock(this WebApplicationFactory<Program> factory)
        => factory.WithWebHostBuilder(app => app
            .ConfigureServices(services => 
            {
                services.AddSingleton<BadgeValidator, MockBadgeValidator>();
            })
            .ConfigureAppConfiguration((_, config) =>
            {
                config.Sources.Clear();
                config.AddInMemoryCollection(_configuration!);
            }));


    public static IServiceScope CreateMultitenancyScope(this WebApplicationFactory<Program> factory, string group)
        => factory.Services.CreateMultitenancyScope(group);

    public static AuthenticationHeaderValue MockValidMemberAuthorizationHeader(this WebApplicationFactory<Program> factory, string group)
    {
        var claims = new Claim[]
        {
            new Claim("badge", "member"),
            new Claim("client", Client),
            new Claim("group", group),
            new Claim("member", Member)
        };

        var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, ClaimsJsonSerializerOptions.Options);

        var bearer = WebEncoders.Base64UrlEncode(claimsSerialized);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }

    public static AuthenticationHeaderValue MockValidTickerAuthorizationHeader(this WebApplicationFactory<Program> factory, string group)
    {
        var claims = new Claim[]
        {
            new Claim("badge", "ticker"),
            new Claim("client", Client),
            new Claim("group", group),
            new Claim("mail", Mail)
        };

        var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, ClaimsJsonSerializerOptions.Options);

        var bearer = WebEncoders.Base64UrlEncode(claimsSerialized);

        return new AuthenticationHeaderValue("Bearer", bearer);

    }
}