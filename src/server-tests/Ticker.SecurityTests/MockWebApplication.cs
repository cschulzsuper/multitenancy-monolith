using ChristianSchulz.MultitenancyMonolith.Application.Ticker;
using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Ticker.ConcreteValidators;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker.Security;
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
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Security.Claims;
using Xunit;
using Xunit.Abstractions;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]
internal static class MockWebApplication
{
    public const int MockAdmin = 1;
    public const int MockIdentity = 2;
    public const int MockDemo = 3;
    public const int MockChief = 4;
    public const int MockChiefObserver = 5;
    public const int MockMember = 6;
    public const int MockMemberObserver = 7;
    public const int MockTicker = 8;

    public const string ClientName = "security-tests";

    public const string AuthenticationIdentityAdmin = "admin";
    public const string AuthenticationIdentityIdentity = "identity";
    public const string AuthenticationIdentityDemo = "demo";

    public const string AccountGroup = "group";
    public const string AccountGroupChief = "chief-member";
    public const string AccountGroupMember = "member";

    public const string ConfirmedMailAddress = "confirmed@localhost";
    public const string ConfirmedSecret = "confirmed";
    public const string ConfirmedDisplayName = "Confirmed";
    public static readonly Guid ConfirmedSecretToken = Guid.NewGuid();

    public const string InvalidMailAddress = "invalid@localhost";
    public const string InvalidSecret = "invalid";
    public const string InvalidDisplayName = "Invalid";
    public static readonly Guid InvalidSecretToken = Guid.NewGuid();

    public const string PendingMailAddress = "pending@localhost";
    public const string PendingSecret = "pending";
    public const string PendingDisplayName = "Pending";
    public static readonly Guid PendingSecretToken = Guid.NewGuid();

    public const string ResetMailAddress = "reset@localhost";
    public const string ResetSecret = "reset";
    public const string ResetDisplayName = "Reset";
    public static readonly Guid ResetSecretToken = Guid.NewGuid();

    public static readonly IDictionary<string, Guid> SecretTokens = new Dictionary<string, Guid>
    {
        [ConfirmedMailAddress] = ConfirmedSecretToken,
        [InvalidMailAddress] = InvalidSecretToken,
        [PendingMailAddress] = PendingSecretToken,
        [ResetMailAddress] = ResetSecretToken,
    };

    private static readonly IDictionary<string, string> _configuration = new Dictionary<string, string>()
    {
        {"ServiceMappings:0:UniqueName", "server"},
        {"ServiceMappings:0:Url", "http://localhost"},
        {"ServiceMappings:1:UniqueName", "ticker"},
        {"ServiceMappings:1:Url", "http://localhost"},

        {"AdmissionServer:Service", ClientName},

        {"AllowedClients:0:Service", "swagger"},
        {"AllowedClients:0:Scopes:0", "swagger-json"},
        {"AllowedClients:0:Scopes:1", "endpoints"},
        {"AllowedClients:1:Service", "security-tests"},
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
            .ConfigureServices(services =>
            {
                services.Configure<BearerTokenOptions>(BearerTokenDefaults.AuthenticationScheme, options =>
                {
                    options.Events.OnMessageReceived = BearerTokenMessageHandler.Handle<MockBearerTokenValidator>;
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

    public static AuthenticationHeaderValue MockValidAuthorizationHeader(this WebApplicationFactory<Program> factory, int mock, string client = ClientName)
        => mock switch
        {
            MockAdmin => factory.MockValidAdminAuthorizationHeader(client),
            MockIdentity => factory.MockValidIdentityAuthorizationHeader(client),
            MockDemo => factory.MockValidDemoAuthorizationHeader(client),
            MockChief => factory.MockValidChiefAuthorizationHeader(client),
            MockChiefObserver => factory.MockValidChiefObserverAuthorizationHeader(client),
            MockMember => factory.MockValidMemberAuthorizationHeader(client),
            MockMemberObserver => factory.MockValidMemberObserverAuthorizationHeader(client),
            MockTicker => factory.MockValidTickerAuthorizationHeader(client),
            _ => throw new UnreachableException("Mock not found!")
        };

    private static AuthenticationHeaderValue MockValidAdminAuthorizationHeader(this WebApplicationFactory<Program> factory, string client)
    {
        var claims = new Claim[]
        {
            new Claim("type", "identity"),
            new Claim("client", client),
            new Claim("identity", AuthenticationIdentityAdmin)
        };

        var token = factory.ProtectClaims(claims);

        return new AuthenticationHeaderValue("Bearer", token);
    }

    private static AuthenticationHeaderValue MockValidIdentityAuthorizationHeader(this WebApplicationFactory<Program> factory, string client)
    {
        var claims = new Claim[]
        {
            new Claim("type", "identity"),
            new Claim("client", client),
            new Claim("identity", AuthenticationIdentityIdentity)
        };

        var token = factory.ProtectClaims(claims);

        return new AuthenticationHeaderValue("Bearer", token);
    }

    private static AuthenticationHeaderValue MockValidDemoAuthorizationHeader(this WebApplicationFactory<Program> factory, string client)
    {
        var claims = new Claim[]
        {
            new Claim("type", "identity"),
            new Claim("client", client),
            new Claim("identity", AuthenticationIdentityDemo)
        };

        var token = factory.ProtectClaims(claims);

        return new AuthenticationHeaderValue("Bearer", token);
    }

    private static AuthenticationHeaderValue MockValidChiefAuthorizationHeader(this WebApplicationFactory<Program> factory, string client)
    {
        var claims = new Claim[]
        {
            new Claim("type", "member"),
            new Claim("client", client),
            new Claim("identity", AuthenticationIdentityIdentity),
            new Claim("group", AccountGroup),
            new Claim("member", AccountGroupChief)
        };

        var token = factory.ProtectClaims(claims);

        return new AuthenticationHeaderValue("Bearer", token);
    }

    private static AuthenticationHeaderValue MockValidChiefObserverAuthorizationHeader(this WebApplicationFactory<Program> factory, string client)
    {
        var claims = new Claim[]
        {
            new Claim("type", "member"),
            new Claim("client", client),
            new Claim("identity", AuthenticationIdentityDemo),
            new Claim("group", AccountGroup),
            new Claim("member", AccountGroupChief)
        };

        var token = factory.ProtectClaims(claims);

        return new AuthenticationHeaderValue("Bearer", token);
    }

    private static AuthenticationHeaderValue MockValidMemberAuthorizationHeader(this WebApplicationFactory<Program> factory, string client)
    {
        var claims = new Claim[]
        {
            new Claim("type", "member"),
            new Claim("client", client),
            new Claim("identity", AuthenticationIdentityIdentity),
            new Claim("group", AccountGroup),
            new Claim("member", AccountGroupMember)
        };

        var token = factory.ProtectClaims(claims);

        return new AuthenticationHeaderValue("Bearer", token);
    }

    private static AuthenticationHeaderValue MockValidMemberObserverAuthorizationHeader(this WebApplicationFactory<Program> factory, string client)
    {
        var claims = new Claim[]
        {
            new Claim("type", "member"),
            new Claim("client", client),
            new Claim("identity", AuthenticationIdentityDemo),
            new Claim("group", AccountGroup),
            new Claim("member", AccountGroupMember)
        };

        var token = factory.ProtectClaims(claims);

        return new AuthenticationHeaderValue("Bearer", token);
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

        var token = factory.ProtectClaims(claims);

        return new AuthenticationHeaderValue("Bearer", token);
    }

    public static AuthenticationHeaderValue MockInvalidAuthorizationHeader(this WebApplicationFactory<Program> factory, int mock)
        => mock switch
        {
            MockAdmin => factory.MockInvalidIdentityAuthorizationHeader(),
            MockIdentity => factory.MockInvalidIdentityAuthorizationHeader(),
            MockDemo => factory.MockInvalidIdentityAuthorizationHeader(),
            MockChief => factory.MockInvalidMemberAuthorizationHeader(),
            MockChiefObserver => factory.MockInvalidMemberAuthorizationHeader(),
            MockMember => factory.MockInvalidMemberAuthorizationHeader(),
            MockMemberObserver => factory.MockInvalidMemberAuthorizationHeader(),
            MockTicker => factory.MockInvalidTickerAuthorizationHeader(),
            _ => throw new UnreachableException("Mock not found!")
        };

    private static AuthenticationHeaderValue MockInvalidIdentityAuthorizationHeader(this WebApplicationFactory<Program> factory)
    {
        var claims = new Claim[]
        {
            new Claim("type", "identity"),
            new Claim("client", ClientName),
            new Claim("identity", "invalid")
        };

        var token = factory.ProtectClaims(claims);

        return new AuthenticationHeaderValue("Bearer", token);
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

        var token = factory.ProtectClaims(claims);

        return new AuthenticationHeaderValue("Bearer", token);
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

        var token = factory.ProtectClaims(claims);

        return new AuthenticationHeaderValue("Bearer", token);
    }
}