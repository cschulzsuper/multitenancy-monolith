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
using Xunit;
using ChristianSchulz.MultitenancyMonolith.Application.Admission;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]
internal static class MockWebApplication
{
    public const int MockIdentity = 1;
    public const int MockMember = 6;
    public const int MockTicker = 8;

    public const string ClientName = "security-tests";

    public const string AuthenticationIdentityIdentity = "identity";
    public const string AuthenticationIdentityIdentityMailAddress = "identity@localhost";
    public const string AuthenticationIdentityIdentitySecret = "secret";

    public const string AccountGroup = "group";
    public const string AccountGroupMember = "default";

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

        {$"SeedData:Ticker:TickerUsers:{AccountGroup}:0:MailAddress", ConfirmedMailAddress},
        {$"SeedData:Ticker:TickerUsers:{AccountGroup}:0:Secret", ConfirmedSecret},
        {$"SeedData:Ticker:TickerUsers:{AccountGroup}:0:SecretState", TickerUserSecretStates.Confirmed},
        {$"SeedData:Ticker:TickerUsers:{AccountGroup}:0:SecretToken", $"{ConfirmedSecretToken}"},
        {$"SeedData:Ticker:TickerUsers:{AccountGroup}:0:DisplayName", ConfirmedDisplayName},

        {$"SeedData:Ticker:TickerUsers:{AccountGroup}:1:MailAddress", InvalidMailAddress},
        {$"SeedData:Ticker:TickerUsers:{AccountGroup}:1:Secret", InvalidSecret},
        {$"SeedData:Ticker:TickerUsers:{AccountGroup}:1:SecretState", TickerUserSecretStates.Invalid},
        {$"SeedData:Ticker:TickerUsers:{AccountGroup}:1:SecretToken", $"{InvalidSecretToken}"},
        {$"SeedData:Ticker:TickerUsers:{AccountGroup}:1:DisplayName", InvalidDisplayName},

        {$"SeedData:Ticker:TickerUsers:{AccountGroup}:2:MailAddress", ResetMailAddress},
        {$"SeedData:Ticker:TickerUsers:{AccountGroup}:2:Secret", ResetSecret},
        {$"SeedData:Ticker:TickerUsers:{AccountGroup}:2:SecretState", TickerUserSecretStates.Reset},
        {$"SeedData:Ticker:TickerUsers:{AccountGroup}:2:SecretToken", $"{ResetSecretToken}"},
        {$"SeedData:Ticker:TickerUsers:{AccountGroup}:2:DisplayName", ResetDisplayName},

        {$"SeedData:Ticker:TickerUsers:{AccountGroup}:3:MailAddress", PendingMailAddress},
        {$"SeedData:Ticker:TickerUsers:{AccountGroup}:3:Secret", PendingSecret},
        {$"SeedData:Ticker:TickerUsers:{AccountGroup}:3:SecretState", TickerUserSecretStates.Pending},
        {$"SeedData:Ticker:TickerUsers:{AccountGroup}:3:SecretToken", $"{PendingSecretToken}"},
        {$"SeedData:Ticker:TickerUsers:{AccountGroup}:3:DisplayName", PendingDisplayName},
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
        => factory.Services.CreateMultitenancyScope(AccountGroup);

    public static AuthenticationHeaderValue MockValidAuthorizationHeader(this WebApplicationFactory<Program> factory, int mock, string client = ClientName)
        => mock switch
        {
            MockIdentity => factory.MockValidIdentityAuthorizationHeader(client),
            MockMember => factory.MockValidMemberAuthorizationHeader(client),
            MockTicker => factory.MockValidTickerAuthorizationHeader(client),
            _ => throw new UnreachableException("Mock not found!")
        };

    private static AuthenticationHeaderValue MockValidIdentityAuthorizationHeader(this WebApplicationFactory<Program> factory, string client)
    {
        var claims = new Claim[]
        {
            new Claim("type", "identity"),
            new Claim("client", client),
            new Claim("identity", AuthenticationIdentityIdentity)
        };

        var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, ClaimsJsonSerializerOptions.Options);

        var bearer = WebEncoders.Base64UrlEncode(claimsSerialized);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }

    private static AuthenticationHeaderValue MockValidMemberAuthorizationHeader(this WebApplicationFactory<Program> factory, string client)
    {
        var claims = new Claim[]
        {
            new Claim("type", "member"),
            new Claim("client", client),
            new Claim("group", AccountGroup),
            new Claim("member", AccountGroupMember)
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
            ClientName = client,
            AccountGroup = AccountGroup,
            Mail = ConfirmedMailAddress,
        };

        using var scope = factory.Services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<ITickerUserVerificationManager>()
            .Set(verificationKey, verification);

        var claims = new Claim[]
        {
            new Claim("type", "ticker"),
            new Claim("client", client),
            new Claim("group", AccountGroup),
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
            MockIdentity => factory.MockInvalidAuthenticationIdentityAuthorizationHeader(),
            MockMember => factory.MockInvalidMemberAuthorizationHeader(),
            MockTicker => factory.MockInvalidTickerAuthorizationHeader(),
            _ => throw new UnreachableException("Mock not found!")
        };

    private static AuthenticationHeaderValue MockInvalidAuthenticationIdentityAuthorizationHeader(this WebApplicationFactory<Program> factory)
    {
        var claims = new Claim[]
        {
            new Claim("type", "identity"),
            new Claim("client", ClientName),
            new Claim("identity", AuthenticationIdentityIdentity)
        };

        var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, ClaimsJsonSerializerOptions.Options);

        var bearer = WebEncoders.Base64UrlEncode(claimsSerialized);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }

    private static AuthenticationHeaderValue MockInvalidMemberAuthorizationHeader(this WebApplicationFactory<Program> factory)
    {
        var claims = new Claim[]
        {
            new Claim("type", "member"),
            new Claim("client", ClientName),
            new Claim("group", AccountGroup),
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
            new Claim("type", "ticker"),
            new Claim("client", ClientName),
            new Claim("group", AccountGroup),
            new Claim("mail", InvalidMailAddress),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, ClaimsJsonSerializerOptions.Options);

        var bearer = WebEncoders.Base64UrlEncode(claimsSerialized);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }
}