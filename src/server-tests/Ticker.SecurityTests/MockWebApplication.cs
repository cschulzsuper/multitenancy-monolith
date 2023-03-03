using ChristianSchulz.MultitenancyMonolith.Application.Ticker;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Ticker.ConcreteValidators;
using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker.Security;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge.Serialization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using ChristianSchulz.MultitenancyMonolith.Shared.Logging;
using Xunit.Abstractions;

internal static class MockWebApplication
{
    public const int MockMember = 1;
    public const int MockTicker = 2;

    public const string Client = "security-tests";

    public const string Group = "group";
    public const string Member = "default";

    public const string ConfirmedMailAddress = "confirmed@localhost";
    public const string ConfirmedSecret = "confirmed";
    public const string ConfirmedDisplayName = "Confirmed";
    public readonly static Guid ConfirmedSecretToken = Guid.NewGuid();

    public const string InvalidMailAddress = "invalid@localhost";
    public const string InvalidSecret = "invalid";
    public const string InvalidDisplayName = "Invalid";
    public readonly static Guid InvalidSecretToken = Guid.NewGuid();

    public const string PendingMailAddress = "pending@localhost";
    public const string PendingSecret = "pending";
    public const string PendingDisplayName = "Pending";
    public readonly static Guid PendingSecretToken = Guid.NewGuid();

    public const string ResetMailAddress = "reset@localhost";
    public const string ResetSecret = "reset";
    public const string ResetDisplayName = "Reset";
    public readonly static Guid ResetSecretToken = Guid.NewGuid();

    public static readonly IDictionary<string, Guid> SecretTokens = new Dictionary<string, Guid>
    {
        [ConfirmedMailAddress] = ConfirmedSecretToken,
        [InvalidMailAddress] = InvalidSecretToken,
        [PendingMailAddress] = PendingSecretToken,
        [ResetMailAddress] = ResetSecretToken,
    };

    private static readonly IDictionary<string, string> _configuration = new Dictionary<string, string>()
    {
        {"AllowedClients:0:UniqueName", "swagger"},
        {"AllowedClients:0:Scopes:0", "swagger-json"},
        {"AllowedClients:0:Scopes:1", "endpoints"},
        {"AllowedClients:1:UniqueName", "security-tests"},
        {"AllowedClients:1:Scopes:1", "endpoints"},

        {$"SeedData:Ticker:TickerUsers:{Group}:0:MailAddress", ConfirmedMailAddress},
        {$"SeedData:Ticker:TickerUsers:{Group}:0:Secret", ConfirmedSecret},
        {$"SeedData:Ticker:TickerUsers:{Group}:0:SecretState", TickerUserSecretStates.Confirmed},
        {$"SeedData:Ticker:TickerUsers:{Group}:0:SecretToken", $"{ConfirmedSecretToken}"},
        {$"SeedData:Ticker:TickerUsers:{Group}:0:DisplayName", ConfirmedDisplayName},

        {$"SeedData:Ticker:TickerUsers:{Group}:1:MailAddress", InvalidMailAddress},
        {$"SeedData:Ticker:TickerUsers:{Group}:1:Secret", InvalidSecret},
        {$"SeedData:Ticker:TickerUsers:{Group}:1:SecretState", TickerUserSecretStates.Invalid},
        {$"SeedData:Ticker:TickerUsers:{Group}:1:SecretToken", $"{InvalidSecretToken}"},
        {$"SeedData:Ticker:TickerUsers:{Group}:1:DisplayName", InvalidDisplayName},

        {$"SeedData:Ticker:TickerUsers:{Group}:2:MailAddress", ResetMailAddress},
        {$"SeedData:Ticker:TickerUsers:{Group}:2:Secret", ResetSecret},
        {$"SeedData:Ticker:TickerUsers:{Group}:2:SecretState", TickerUserSecretStates.Reset},
        {$"SeedData:Ticker:TickerUsers:{Group}:2:SecretToken", $"{ResetSecretToken}"},
        {$"SeedData:Ticker:TickerUsers:{Group}:2:DisplayName", ResetDisplayName},

        {$"SeedData:Ticker:TickerUsers:{Group}:3:MailAddress", PendingMailAddress},
        {$"SeedData:Ticker:TickerUsers:{Group}:3:Secret", PendingSecret},
        {$"SeedData:Ticker:TickerUsers:{Group}:3:SecretState", TickerUserSecretStates.Pending},
        {$"SeedData:Ticker:TickerUsers:{Group}:3:SecretToken", $"{PendingSecretToken}"},
        {$"SeedData:Ticker:TickerUsers:{Group}:3:DisplayName", PendingDisplayName},
    };

    public static WebApplicationFactory<Program> Mock(this WebApplicationFactory<Program> factory, ITestOutputHelper? output = null)
        => factory.WithWebHostBuilder(app => app
            .UseEnvironment("Staging")
            .ConfigureServices(services => { services.AddSingleton<BadgeValidator, MockBadgeValidator>(); })
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
        => factory.Services.CreateMultitenancyScope(Group);

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
            Mail = ConfirmedMailAddress,
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
            new Claim("mail", ConfirmedMailAddress),
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
            new Claim("mail", InvalidMailAddress),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, ClaimsJsonSerializerOptions.Options);

        var bearer = WebEncoders.Base64UrlEncode(claimsSerialized);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }
}