using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker.Security;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge.Serialization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

internal static class MockWebApplication
{
    public const string Client = "integration-tests";

    public const string Group = "group";
    public const string Member = "member";

    public const string TickerUserMail = "default@localhost";
    public const string TickerUserSecret = "default";
    public readonly static Guid TickerUserSecretToken = Guid.NewGuid();

    private static readonly IDictionary<string, string> _configuration = new Dictionary<string, string>()
    {
        {"AllowedClients:0:UniqueName", Client},
        {"AllowedClients:0:Scopes:0", "swagger-json"},
        {"AllowedClients:0:Scopes:1", "endpoints"},
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


    public static IServiceScope CreateMultitenancyScope(this WebApplicationFactory<Program> factory)
        => factory.Services.CreateMultitenancyScope(Group);

    public static AuthenticationHeaderValue MockValidMemberAuthorizationHeader(this WebApplicationFactory<Program> factory)
    {
        var claims = new Claim[]
        {
            new Claim("badge", "member"),
            new Claim("client", Client),
            new Claim("group", Group),
            new Claim("member", Member)
        };

        var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, ClaimsJsonSerializerOptions.Options);

        var bearer = WebEncoders.Base64UrlEncode(claimsSerialized);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }

    public static AuthenticationHeaderValue MockValidTickerAuthorizationHeader(this WebApplicationFactory<Program> factory)
    {
        var claims = new Claim[]
        {
            new Claim("badge", "ticker"),
            new Claim("client", Client),
            new Claim("group", Group),
            new Claim("mail", TickerUserMail)
        };

        var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, ClaimsJsonSerializerOptions.Options);

        var bearer = WebEncoders.Base64UrlEncode(claimsSerialized);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }
}