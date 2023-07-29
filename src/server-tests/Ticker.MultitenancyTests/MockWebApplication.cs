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
using Xunit;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]
internal static class MockWebApplication
{
    public const string ClientName = "multitenancy-tests";

    public const string AccountGroup1 = "group-1";
    public const string AccountGroup2 = "group-2";
    public const string Identity = "default";
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
            new Claim("type", "member"),
            new Claim("client", ClientName),
            new Claim("identity", Identity),
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
            new Claim("type", "ticker"),
            new Claim("client", ClientName),
            new Claim("group", group),
            new Claim("mail", Mail)
        };

        var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, ClaimsJsonSerializerOptions.Options);

        var bearer = WebEncoders.Base64UrlEncode(claimsSerialized);

        return new AuthenticationHeaderValue("Bearer", bearer);

    }
}