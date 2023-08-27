using ChristianSchulz.MultitenancyMonolith.Application.Access;
using ChristianSchulz.MultitenancyMonolith.Application.Admission;
using ChristianSchulz.MultitenancyMonolith.Server;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Hosting;
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

    public const string Client = "security-tests";

    public const string AuthenticationIdentityAdmin = "admin";
    public const string AuthenticationIdentityAdminMailAddress = "admin@localhost";
    public const string AuthenticationIdentityAdminSecret = "secret";

    public const string AuthenticationIdentityIdentity = "identity";
    public const string AuthenticationIdentityIdentityMailAddress = "identity@localhost";
    public const string AuthenticationIdentityIdentitySecret = "secret";

    public const string AuthenticationIdentityDemo = "demo";
    public const string AuthenticationIdentityDemoMailAddress = "demo@localhost";
    public const string AuthenticationIdentityDemoSecret = "secret";

    public const string AccountGroup = "group";
    
    public const string AccountGroupChief = "chief-member";
    public const string AccountGroupChiefMailAddress = "chief@localhost";

    public const string AccountGroupMember = "member";
    public const string AccountGroupMemberMailAddress = "member@localhost";

    private static readonly IDictionary<string, string> _configuration = new Dictionary<string, string>()
    {
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
        {"SeedData:2:Resource:Secret", AuthenticationIdentityDemoSecret},
        {"SeedData:3:Scheme", "access/account-groups"},
        {"SeedData:3:Resource:UniqueName", AccountGroup},
        {"SeedData:4:Scheme", "access/account-members"},
        {"SeedData:4:Resource:AccountGroup", AccountGroup},
        {"SeedData:4:Resource:UniqueName", AccountGroupChief},
        {"SeedData:4:Resource:MailAddress", AccountGroupChiefMailAddress},
        {"SeedData:4:Resource:AuthenticationIdentities:0", AuthenticationIdentityIdentity},
        {"SeedData:4:Resource:AuthenticationIdentities:1", AuthenticationIdentityDemo},
        {"SeedData:5:Scheme", "access/account-members"},
        {"SeedData:5:Resource:AccountGroup", AccountGroup},
        {"SeedData:5:Resource:UniqueName", AccountGroupMember},
        {"SeedData:5:Resource:MailAddress", AccountGroupMemberMailAddress},
        {"SeedData:5:Resource:AuthenticationIdentities:0", AuthenticationIdentityIdentity},
        {"SeedData:5:Resource:AuthenticationIdentities:1", AuthenticationIdentityDemo},
    };

    public static WebApplicationFactory<Program> Mock(this WebApplicationFactory<Program> factory)
        => factory.WithWebHostBuilder(app => app
            .UseEnvironment("Staging")
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

    public static AuthenticationHeaderValue MockValidAuthorizationHeader(this WebApplicationFactory<Program> factory, int mock, string client = Client)
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
        var verification = Guid.NewGuid().ToByteArray();

        var verificationKey = new AuthenticationIdentityVerificationKey
        {
            ClientName = client,
            AuthenticationIdentity = AuthenticationIdentityAdmin
        };

        using var scope = factory.Services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IAuthenticationIdentityVerificationManager>()
            .Set(verificationKey, verification);

        var claims = new Claim[]
        {
            new Claim("type", "identity"),
            new Claim("client", client),
            new Claim("identity", AuthenticationIdentityAdmin),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var token = factory.ProtectClaims(claims);

        return new AuthenticationHeaderValue("Bearer", token);
    }

    private static AuthenticationHeaderValue MockValidIdentityAuthorizationHeader(this WebApplicationFactory<Program> factory, string client)
    {
        var verification = Guid.NewGuid().ToByteArray();

        var verificationKey = new AuthenticationIdentityVerificationKey
        {
            ClientName = client,
            AuthenticationIdentity = AuthenticationIdentityIdentity
        };

        using var scope = factory.Services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IAuthenticationIdentityVerificationManager>()
            .Set(verificationKey, verification);

        var claims = new Claim[]
        {
            new Claim("type", "identity"),
            new Claim("client", client),
            new Claim("identity", AuthenticationIdentityIdentity),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var token = factory.ProtectClaims(claims);

        return new AuthenticationHeaderValue("Bearer", token);
    }

    private static AuthenticationHeaderValue MockValidDemoAuthorizationHeader(this WebApplicationFactory<Program> factory, string client)
    {
        var verification = Guid.NewGuid().ToByteArray();

        var verificationKey = new AuthenticationIdentityVerificationKey
        {
            ClientName = client,
            AuthenticationIdentity = AuthenticationIdentityDemo
        };

        using var scope = factory.Services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IAuthenticationIdentityVerificationManager>()
            .Set(verificationKey, verification);

        var claims = new Claim[]
        {
            new Claim("type", "identity"),
            new Claim("client", client),
            new Claim("identity", AuthenticationIdentityDemo),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var token = factory.ProtectClaims(claims);

        return new AuthenticationHeaderValue("Bearer", token);
    }

    private static AuthenticationHeaderValue MockValidChiefAuthorizationHeader(this WebApplicationFactory<Program> factory, string client)
    {
        var verification = Guid.NewGuid().ToByteArray();

        var verificationKey = new AccountMemberVerificationKey
        {
            ClientName = client,
            AuthenticationIdentity = AuthenticationIdentityIdentity,
            AccountGroup = AccountGroup,
            AccountMember = AccountGroupChief,
        };

        using var scope = factory.Services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IAccountMemberVerificationManager>()
            .Set(verificationKey, verification);

        var claims = new Claim[]
        {
            new Claim("type", "member"),
            new Claim("client", client),
            new Claim("identity", AuthenticationIdentityIdentity),
            new Claim("group", AccountGroup),
            new Claim("member", AccountGroupChief),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var token = factory.ProtectClaims(claims);

        return new AuthenticationHeaderValue("Bearer", token);
    }

    private static AuthenticationHeaderValue MockValidChiefObserverAuthorizationHeader(this WebApplicationFactory<Program> factory, string client)
    {
        var verification = Guid.NewGuid().ToByteArray();

        var verificationKey = new AccountMemberVerificationKey
        {
            ClientName = client,
            AuthenticationIdentity = AuthenticationIdentityDemo,
            AccountGroup = AccountGroup,
            AccountMember = AccountGroupChief,
        };

        using var scope = factory.Services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IAccountMemberVerificationManager>()
            .Set(verificationKey, verification);

        var claims = new Claim[]
        {
            new Claim("type", "member"),
            new Claim("client", client),
            new Claim("identity", AuthenticationIdentityDemo),
            new Claim("group", AccountGroup),
            new Claim("member", AccountGroupChief),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var token = factory.ProtectClaims(claims);

        return new AuthenticationHeaderValue("Bearer", token);
    }

    private static AuthenticationHeaderValue MockValidMemberAuthorizationHeader(this WebApplicationFactory<Program> factory, string client)
    {
        var verification = Guid.NewGuid().ToByteArray();

        var verificationKey = new AccountMemberVerificationKey
        {
            ClientName = client,
            AuthenticationIdentity = AuthenticationIdentityIdentity,
            AccountGroup = AccountGroup,
            AccountMember = AccountGroupMember,
        };

        using var scope = factory.Services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IAccountMemberVerificationManager>()
            .Set(verificationKey, verification);

        var claims = new Claim[]
        {
            new Claim("type", "member"),
            new Claim("client", client),
            new Claim("identity", AuthenticationIdentityIdentity),
            new Claim("group", AccountGroup),
            new Claim("member", AccountGroupMember),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var token = factory.ProtectClaims(claims);

        return new AuthenticationHeaderValue("Bearer", token);
    }

    private static AuthenticationHeaderValue MockValidMemberObserverAuthorizationHeader(this WebApplicationFactory<Program> factory, string client)
    {
        var verification = Guid.NewGuid().ToByteArray();

        var verificationKey = new AccountMemberVerificationKey
        {
            ClientName = client,
            AuthenticationIdentity = AuthenticationIdentityDemo,
            AccountGroup = AccountGroup,
            AccountMember = AccountGroupMember,
        };

        using var scope = factory.Services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IAccountMemberVerificationManager>()
            .Set(verificationKey, verification);

        var claims = new Claim[]
        {
            new Claim("type", "member"),
            new Claim("client", client),
            new Claim("identity", AuthenticationIdentityDemo),
            new Claim("group", AccountGroup),
            new Claim("member", AccountGroupMember),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var token = factory.ProtectClaims(claims);

        return new AuthenticationHeaderValue("Bearer", token);
    }

    public static AuthenticationHeaderValue MockInvalidAuthorizationHeader(this WebApplicationFactory<Program> factory, int mock)
        => mock switch
        {
            MockAdmin => factory.MockInvalidAdminAuthorizationHeader(),
            MockIdentity => factory.MockInvalidAuthenticationIdentityAuthorizationHeader(),
            MockDemo => factory.MockInvalidDemoAuthorizationHeader(),
            MockChief => factory.MockInvalidChiefAuthorizationHeader(),
            MockChiefObserver => factory.MockInvalidChiefObserverAuthorizationHeader(),
            MockMember => factory.MockInvalidMemberAuthorizationHeader(),
            MockMemberObserver => factory.MockInvalidMemberObserverAuthorizationHeader(),
            _ => throw new UnreachableException("Mock not found!")
        };

    private static AuthenticationHeaderValue MockInvalidAdminAuthorizationHeader(this WebApplicationFactory<Program> factory)
    {
        var verification = Guid.NewGuid().ToByteArray();

        var claims = new Claim[]
        {
            new Claim("type", "identity"),
            new Claim("client", Client),
            new Claim("identity", AuthenticationIdentityAdmin),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var token = factory.ProtectClaims(claims);

        return new AuthenticationHeaderValue("Bearer", token);
    }

    private static AuthenticationHeaderValue MockInvalidAuthenticationIdentityAuthorizationHeader(this WebApplicationFactory<Program> factory)
    {
        var verification = Guid.NewGuid().ToByteArray();

        var claims = new Claim[]
        {
            new Claim("type", "identity"),
            new Claim("client", Client),
            new Claim("identity", AuthenticationIdentityIdentity),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var token = factory.ProtectClaims(claims);

        return new AuthenticationHeaderValue("Bearer", token);
    }

    private static AuthenticationHeaderValue MockInvalidDemoAuthorizationHeader(this WebApplicationFactory<Program> factory)
    {
        var verification = Guid.NewGuid().ToByteArray();

        var claims = new Claim[]
        {
            new Claim("type", "identity"),
            new Claim("client", Client),
            new Claim("identity", AuthenticationIdentityDemo),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var token = factory.ProtectClaims(claims);

        return new AuthenticationHeaderValue("Bearer", token);
    }

    private static AuthenticationHeaderValue MockInvalidChiefAuthorizationHeader(this WebApplicationFactory<Program> factory)
    {
        var verification = Guid.NewGuid().ToByteArray();

        var claims = new Claim[]
        {
            new Claim("type", "member"),
            new Claim("client", Client),
            new Claim("identity", AuthenticationIdentityIdentity),
            new Claim("group", AccountGroup),
            new Claim("member", AccountGroupChief),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var token = factory.ProtectClaims(claims);

        return new AuthenticationHeaderValue("Bearer", token);
    }

    private static AuthenticationHeaderValue MockInvalidChiefObserverAuthorizationHeader(this WebApplicationFactory<Program> factory)
    {
        var verification = Guid.NewGuid().ToByteArray();

        var claims = new Claim[]
        {
            new Claim("type", "member"),
            new Claim("client", Client),
            new Claim("identity", AuthenticationIdentityDemo),
            new Claim("group", AccountGroup),
            new Claim("member", AccountGroupChief),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var token = factory.ProtectClaims(claims);

        return new AuthenticationHeaderValue("Bearer", token);
    }

    private static AuthenticationHeaderValue MockInvalidMemberAuthorizationHeader(this WebApplicationFactory<Program> factory)
    {
        var verification = Guid.NewGuid().ToByteArray();

        var claims = new Claim[]
        {
            new Claim("type", "member"),
            new Claim("client", Client),
            new Claim("identity", AuthenticationIdentityIdentity),
            new Claim("group", AccountGroup),
            new Claim("member", AccountGroupMember),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var token = factory.ProtectClaims(claims);

        return new AuthenticationHeaderValue("Bearer", token);
    }

    private static AuthenticationHeaderValue MockInvalidMemberObserverAuthorizationHeader(this WebApplicationFactory<Program> factory)
    {
        var verification = Guid.NewGuid().ToByteArray();

        var claims = new Claim[]
        {
            new Claim("type", "member"),
            new Claim("client", Client),
            new Claim("identity", AuthenticationIdentityDemo),
            new Claim("group", AccountGroup),
            new Claim("member", AccountGroupMember),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var token = factory.ProtectClaims(claims);

        return new AuthenticationHeaderValue("Bearer", token);
    }

}