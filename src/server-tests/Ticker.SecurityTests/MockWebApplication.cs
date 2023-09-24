﻿using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Ticker.ConcreteValidators;
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

        {"SeedData:0:Scheme", "ticker/ticker-users"},
        {"SeedData:0:Resource:AccountGroup", AccountGroup},
        {"SeedData:0:Resource:MailAddress", ConfirmedMailAddress},
        {"SeedData:0:Resource:Secret", ConfirmedSecret},
        {"SeedData:0:Resource:SecretState", TickerUserSecretStates.Confirmed},
        {"SeedData:0:Resource:SecretToken", $"{ConfirmedSecretToken}"},
        {"SeedData:0:Resource:DisplayName", ConfirmedDisplayName},

        {"SeedData:1:Scheme", "ticker/ticker-users"},
        {"SeedData:1:Resource:AccountGroup", AccountGroup},
        {"SeedData:1:Resource:MailAddress", InvalidMailAddress},
        {"SeedData:1:Resource:Secret", InvalidSecret},
        {"SeedData:1:Resource:SecretState", TickerUserSecretStates.Invalid},
        {"SeedData:1:Resource:SecretToken", $"{InvalidSecretToken}"},
        {"SeedData:1:Resource:DisplayName", InvalidDisplayName},

        {"SeedData:2:Scheme", "ticker/ticker-users"},
        {"SeedData:2:Resource:AccountGroup", AccountGroup},
        {"SeedData:2:Resource:MailAddress", ResetMailAddress},
        {"SeedData:2:Resource:Secret", ResetSecret},
        {"SeedData:2:Resource:SecretState", TickerUserSecretStates.Reset},
        {"SeedData:2:Resource:SecretToken", $"{ResetSecretToken}"},
        {"SeedData:2:Resource:DisplayName", ResetDisplayName},

        {"SeedData:3:Scheme", "ticker/ticker-users"},
        {"SeedData:3:Resource:AccountGroup", AccountGroup},
        {"SeedData:3:Resource:MailAddress", PendingMailAddress},
        {"SeedData:3:Resource:Secret", PendingSecret},
        {"SeedData:3:Resource:SecretState", TickerUserSecretStates.Pending},
        {"SeedData:3:Resource:SecretToken", $"{PendingSecretToken}"},
        {"SeedData:3:Resource:DisplayName", PendingDisplayName},
    };

    public static WebApplicationFactory<Program> Mock(this WebApplicationFactory<Program> factory, ITestOutputHelper? output = null)
        => factory.WithWebHostBuilder(app => app
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
        var claims = new Claim[]
        {
            new Claim("type", "ticker"),
            new Claim("client", client),
            new Claim("group", AccountGroup),
            new Claim("mail", ConfirmedMailAddress)
        };

        var token = factory.ProtectClaims(claims);

        return new AuthenticationHeaderValue("Bearer", token);
    }

    public static AuthenticationHeaderValue MockInvalidAuthorizationHeader(this WebApplicationFactory<Program> factory)
    {
        var claims = new Claim[]
        {
            new Claim("type", "invalid")
        };

        var token = factory.ProtectClaims(claims);

        return new AuthenticationHeaderValue("Bearer", token);
    }
}