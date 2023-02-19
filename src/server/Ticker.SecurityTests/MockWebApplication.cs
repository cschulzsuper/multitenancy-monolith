using ChristianSchulz.MultitenancyMonolith.Application.Ticker;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker.Security;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge.Serialization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

internal static class MockWebApplication
{
    public const int MockMember = 1;
    public const int MockTicker = 2;

    public const string Client = "security-tests";

    public const string Group = "group";
    public const string Member = "default";

    public const string TickerUserMailAddress = "default@localhost";
    public const string TickerUserSecret = "default";
    public const string TickerUserDisplayName = "Default";

    private static readonly IDictionary<string, string> _configuration = new Dictionary<string, string>()
    {
        {"AllowedClients:0:UniqueName", "swagger"},
        {"AllowedClients:0:Scopes:0", "swagger-json"},
        {"AllowedClients:0:Scopes:1", "endpoints"},
        {"AllowedClients:1:UniqueName", "security-tests"},
        {"AllowedClients:1:Scopes:1", "endpoints"},

        {$"SeedData:Ticker:TickerUsers:{Group}:0:MailAddress", TickerUserMailAddress},
        {$"SeedData:Ticker:TickerUsers:{Group}:0:Secret", TickerUserSecret},
        {$"SeedData:Ticker:TickerUsers:{Group}:0:SecretState", "confirmed"},
        {$"SeedData:Ticker:TickerUsers:{Group}:0:SecretToken", $"{Guid.NewGuid()}"},
        {$"SeedData:Ticker:TickerUsers:{Group}:0:DisplayName", TickerUserDisplayName},
    };

    public static WebApplicationFactory<Program> Mock(this WebApplicationFactory<Program> factory)
        => factory.WithWebHostBuilder(app => app
            .UseEnvironment("Staging")
            .ConfigureServices(services =>
            {
                services.AddSingleton<BadgeValidator,MockBadgeValidator>();
            })
            .ConfigureAppConfiguration((_, config) =>
            {
                config.Sources.Clear();
                config.AddInMemoryCollection(_configuration!);
            }));

    public static AuthenticationHeaderValue MockValidAuthorizationHeader(this WebApplicationFactory<Program> factory, int mock, string client = Client)
        => mock switch
        {
            MockMember => factory.MockValidMemberAuthorizationHeader(client),
            MockTicker => factory.MockValidTickerAuthorizationHeader(client),
            _ => throw new UnreachableException("Mock not found!")
        };

    private static AuthenticationHeaderValue MockValidMemberAuthorizationHeader(this WebApplicationFactory<Program> factory, string client)
    {
        var claims = new Claim[]
        {
            new Claim("badge", "member"),
            new Claim("client", client),
            new Claim("group", Group),
            new Claim("member", Member)
        };

        var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, ClaimsJsonSerializerOptions.Options);

        var bearer = WebEncoders.Base64UrlEncode(claimsSerialized);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }

    private static AuthenticationHeaderValue MockValidTickerAuthorizationHeader(this WebApplicationFactory<Program> factory, string client)
    {
        var verification = Guid.NewGuid().ToByteArray();

        var verificationKey = new TickerUserVerificationKey
        {
            Client = client,
            Group = Group,
            Mail = TickerUserMailAddress,
        };

        using var scope = factory.Services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<ITickerUserVerificationManager>()
            .Set(verificationKey, verification);       

        var claims = new Claim[]
        {
            new Claim("badge", "ticker"),
            new Claim("client", client),
            new Claim("group", Group),
            new Claim("mail", TickerUserMailAddress),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, ClaimsJsonSerializerOptions.Options);

        var bearer = WebEncoders.Base64UrlEncode(claimsSerialized);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }

    public static AuthenticationHeaderValue MockInvalidAuthorizationHeader(this WebApplicationFactory<Program> factory, int mock)
        => mock switch
        {
            MockMember => factory.MockInvalidMemberAuthorizationHeader(),
            MockTicker => factory.MockInvalidTickerAuthorizationHeader(),
            _ => throw new UnreachableException("Mock not found!")
        };

    private static AuthenticationHeaderValue MockInvalidMemberAuthorizationHeader(this WebApplicationFactory<Program> factory)
    {
        var claims = new Claim[]
        {
            new Claim("badge", "member"),
            new Claim("client", Client),
            new Claim("group", Group),
            new Claim("member", "invalid")
        };

        var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, ClaimsJsonSerializerOptions.Options);

        var bearer = WebEncoders.Base64UrlEncode(claimsSerialized);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }

    private static AuthenticationHeaderValue MockInvalidTickerAuthorizationHeader(this WebApplicationFactory<Program> factory)
    {
        var verification = Guid.NewGuid().ToByteArray();

        var claims = new Claim[]
        {
            new Claim("badge", "ticker"),
            new Claim("client", Client),
            new Claim("group", Group),
            new Claim("mail", TickerUserMailAddress),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, ClaimsJsonSerializerOptions.Options);

        var bearer = WebEncoders.Base64UrlEncode(claimsSerialized);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }
}