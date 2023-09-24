using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Admission.ConcreteValidators;
using ChristianSchulz.MultitenancyMonolith.Server;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Security.Claims;
using Xunit;

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

    public const string ClientName = "security-tests";

    public const string AuthenticationIdentityAdmin = "admin";
    public const string AuthenticationIdentityAdminMailAddress = "admin@localhost";
    public const string AuthenticationIdentityAdminSecret = "secret";

    public const string AuthenticationIdentityIdentity = "identity";
    public const string AuthenticationIdentityIdentityMailAddress = "identity@localhost";
    public const string AuthenticationIdentityIdentitySecret = "secret";

    public const string AuthenticationIdentityDemo = "demo";
    public const string AuthenticationIdentityDemoMailAddress = "demo@localhost";

    public const string MaintenanceSecret = "default";

    public const string AccountGroup = "group";
    
    public const string AccountGroupChief = "chief-member";
    public const string AccountGroupChiefMailAddress = "chief@localhost";

    public const string AccountGroupMember = "member";
    public const string AccountGroupMemberMailAddress = "member@localhost";

    private static readonly IDictionary<string, string> _configuration = new Dictionary<string, string>()
    {
        {"MaintenanceSecret", MaintenanceSecret},

        {"AllowedClients:0:Service", "security-tests"},
        {"AllowedClients:0:Scopes:1", "endpoints"},
        {"AllowedClients:1:Service", "swagger"},
        {"AllowedClients:1:Scopes:0", "swagger-json"},
        {"AllowedClients:1:Scopes:1", "endpoints"},

        {"SeedData:0:Scheme", "admission/authentication-identities"},
        {"SeedData:0:Resource:UniqueName", AuthenticationIdentityAdmin},
        {"SeedData:0:Resource:MailAddress", AuthenticationIdentityAdminMailAddress},
        {"SeedData:0:Resource:Secret", AuthenticationIdentityAdminSecret},
        {"SeedData:1:Scheme", "admission/authentication-identities"},
        {"SeedData:1:Resource:UniqueName", AuthenticationIdentityIdentity},
        {"SeedData:1:Resource:MailAddress", AuthenticationIdentityIdentityMailAddress},
        {"SeedData:1:Resource:Secret", AuthenticationIdentityIdentitySecret},
        {"SeedData:2:Scheme", "admission/authentication-identities"},
        {"SeedData:2:Resource:UniqueName", AuthenticationIdentityDemo},
        {"SeedData:2:Resource:MailAddress", AuthenticationIdentityDemoMailAddress},
        {"SeedData:2:Resource:Secret", $"{Guid.NewGuid()}"},

        {"SeedData:3:Scheme", "admission/authentication-identity-authentication-methods"},
        {"SeedData:3:Resource:AuthenticationIdentity", AuthenticationIdentityDemo},
        {"SeedData:3:Resource:ClientName", ClientName},
        {"SeedData:3:Resource:AuthenticationMethod", AuthenticationMethods.Anonymouse},

        {"SeedData:4:Scheme", "admission/authentication-identity-authentication-methods"},
        {"SeedData:4:Resource:AuthenticationIdentity", AuthenticationIdentityAdmin},
        {"SeedData:4:Resource:ClientName", ClientName},
        {"SeedData:4:Resource:AuthenticationMethod", AuthenticationMethods.Maintenance},

        {"SeedData:5:Scheme", "access/account-groups"},
        {"SeedData:5:Resource:UniqueName", AccountGroup},

        {"SeedData:6:Scheme", "access/account-members"},
        {"SeedData:6:Resource:AccountGroup", AccountGroup},
        {"SeedData:6:Resource:UniqueName", AccountGroupChief},
        {"SeedData:6:Resource:MailAddress", AccountGroupChiefMailAddress},
        {"SeedData:6:Resource:AuthenticationIdentities:0", AuthenticationIdentityIdentity},
        {"SeedData:6:Resource:AuthenticationIdentities:1", AuthenticationIdentityDemo},

        {"SeedData:7:Scheme", "access/account-members"},
        {"SeedData:7:Resource:AccountGroup", AccountGroup},
        {"SeedData:7:Resource:UniqueName", AccountGroupMember},
        {"SeedData:7:Resource:MailAddress", AccountGroupMemberMailAddress},
        {"SeedData:7:Resource:AuthenticationIdentities:0", AuthenticationIdentityIdentity},
        {"SeedData:7:Resource:AuthenticationIdentities:1", AuthenticationIdentityDemo},
    };

    public static WebApplicationFactory<Program> Mock(this WebApplicationFactory<Program> factory)
        => factory.WithWebHostBuilder(app => app
            .ConfigureAppConfiguration((_, config) =>
            {
                config.Sources.Clear();
                config.AddInMemoryCollection(_configuration!);
            }));

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