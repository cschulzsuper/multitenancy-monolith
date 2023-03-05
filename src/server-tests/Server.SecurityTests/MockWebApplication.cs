using ChristianSchulz.MultitenancyMonolith.Application.Access;
using ChristianSchulz.MultitenancyMonolith.Application.Admission;
using ChristianSchulz.MultitenancyMonolith.Server;
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
        {"AllowedClients:0:UniqueName", "swagger"},
        {"AllowedClients:0:Scopes:0", "swagger-json"},
        {"AllowedClients:0:Scopes:1", "endpoints"},
        {"AllowedClients:1:UniqueName", "endpoint-tests"},
        {"AllowedClients:1:Scopes:1", "endpoints"},
        {"AllowedClients:2:UniqueName", "security-tests"},
        {"AllowedClients:2:Scopes:1", "endpoints"},

        {"SeedData:Admission:AuthenticationIdentities:0:UniqueName", AuthenticationIdentityAdmin},
        {"SeedData:Admission:AuthenticationIdentities:0:MailAddress", AuthenticationIdentityAdminMailAddress},
        {"SeedData:Admission:AuthenticationIdentities:0:Secret", AuthenticationIdentityAdminSecret},
        {"SeedData:Admission:AuthenticationIdentities:1:UniqueName", AuthenticationIdentityIdentity},
        {"SeedData:Admission:AuthenticationIdentities:1:MailAddress", AuthenticationIdentityIdentityMailAddress},
        {"SeedData:Admission:AuthenticationIdentities:1:Secret", AuthenticationIdentityIdentitySecret},
        {"SeedData:Admission:AuthenticationIdentities:2:UniqueName", AuthenticationIdentityDemo},
        {"SeedData:Admission:AuthenticationIdentities:2:MailAddress", AuthenticationIdentityDemoMailAddress},
        {"SeedData:Admission:AuthenticationIdentities:2:Secret", AuthenticationIdentityDemoSecret},
        {$"SeedData:Access:AccountMembers:{AccountGroup}:0:UniqueName", AccountGroupChief},
        {$"SeedData:Access:AccountMembers:{AccountGroup}:0:MailAddress", AccountGroupChiefMailAddress},
        {$"SeedData:Access:AccountMembers:{AccountGroup}:0:AuthenticationIdentities:0:UniqueName", AuthenticationIdentityIdentity},
        {$"SeedData:Access:AccountMembers:{AccountGroup}:0:AuthenticationIdentities:1:UniqueName", AuthenticationIdentityDemo},
        {$"SeedData:Access:AccountMembers:{AccountGroup}:1:UniqueName", AccountGroupMember},
        {$"SeedData:Access:AccountMembers:{AccountGroup}:1:MailAddress", AccountGroupMemberMailAddress},
        {$"SeedData:Access:AccountMembers:{AccountGroup}:1:AuthenticationIdentities:0:UniqueName", AuthenticationIdentityIdentity},
        {$"SeedData:Access:AccountMembers:{AccountGroup}:1:AuthenticationIdentities:1:UniqueName", AuthenticationIdentityDemo},
    };

    public static WebApplicationFactory<Program> Mock(this WebApplicationFactory<Program> factory)
        => factory.WithWebHostBuilder(app => app
            .UseEnvironment("Staging")
            .ConfigureAppConfiguration((_, config) =>
            {
                config.Sources.Clear();
                config.AddInMemoryCollection(_configuration!);
            }));

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

        var verificationKey = new IdentityVerificationKey
        {
            Client = client,
            Identity = AuthenticationIdentityAdmin
        };

        using var scope = factory.Services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IAuthenticationIdentityVerificationManager>()
            .Set(verificationKey, verification);

        var claims = new Claim[]
        {
            new Claim("badge", "identity"),
            new Claim("client", client),
            new Claim("identity", AuthenticationIdentityAdmin),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, ClaimsJsonSerializerOptions.Options);

        var bearer = WebEncoders.Base64UrlEncode(claimsSerialized);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }

    private static AuthenticationHeaderValue MockValidIdentityAuthorizationHeader(this WebApplicationFactory<Program> factory, string client)
    {
        var verification = Guid.NewGuid().ToByteArray();

        var verificationKey = new IdentityVerificationKey
        {
            Client = client,
            Identity = AuthenticationIdentityIdentity
        };

        using var scope = factory.Services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IAuthenticationIdentityVerificationManager>()
            .Set(verificationKey, verification);

        var claims = new Claim[]
        {
            new Claim("badge", "identity"),
            new Claim("client", client),
            new Claim("identity", AuthenticationIdentityIdentity),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, ClaimsJsonSerializerOptions.Options);

        var bearer = WebEncoders.Base64UrlEncode(claimsSerialized);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }

    private static AuthenticationHeaderValue MockValidDemoAuthorizationHeader(this WebApplicationFactory<Program> factory, string client)
    {
        var verification = Guid.NewGuid().ToByteArray();

        var verificationKey = new IdentityVerificationKey
        {
            Client = client,
            Identity = AuthenticationIdentityDemo
        };

        using var scope = factory.Services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IAuthenticationIdentityVerificationManager>()
            .Set(verificationKey, verification);

        var claims = new Claim[]
        {
            new Claim("badge", "identity"),
            new Claim("client", client),
            new Claim("identity", AuthenticationIdentityDemo),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, ClaimsJsonSerializerOptions.Options);

        var bearer = WebEncoders.Base64UrlEncode(claimsSerialized);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }

    private static AuthenticationHeaderValue MockValidChiefAuthorizationHeader(this WebApplicationFactory<Program> factory, string client)
    {
        var verification = Guid.NewGuid().ToByteArray();

        var verificationKey = new AccountMemberVerificationKey
        {
            Client = client,
            Identity = AuthenticationIdentityIdentity,
            Group = AccountGroup,
            Member = AccountGroupChief,
        };

        using var scope = factory.Services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IAccountMemberVerificationManager>()
            .Set(verificationKey, verification);

        var claims = new Claim[]
        {
            new Claim("badge", "member"),
            new Claim("client", client),
            new Claim("identity", AuthenticationIdentityIdentity),
            new Claim("group", AccountGroup),
            new Claim("member", AccountGroupChief),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, ClaimsJsonSerializerOptions.Options);

        var bearer = WebEncoders.Base64UrlEncode(claimsSerialized);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }

    private static AuthenticationHeaderValue MockValidChiefObserverAuthorizationHeader(this WebApplicationFactory<Program> factory, string client)
    {
        var verification = Guid.NewGuid().ToByteArray();

        var verificationKey = new AccountMemberVerificationKey
        {
            Client = client,
            Identity = AuthenticationIdentityDemo,
            Group = AccountGroup,
            Member = AccountGroupChief,
        };

        using var scope = factory.Services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IAccountMemberVerificationManager>()
            .Set(verificationKey, verification);

        var claims = new Claim[]
        {
            new Claim("badge", "member"),
            new Claim("client", client),
            new Claim("identity", AuthenticationIdentityDemo),
            new Claim("group", AccountGroup),
            new Claim("member", AccountGroupChief),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, ClaimsJsonSerializerOptions.Options);

        var bearer = WebEncoders.Base64UrlEncode(claimsSerialized);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }

    private static AuthenticationHeaderValue MockValidMemberAuthorizationHeader(this WebApplicationFactory<Program> factory, string client)
    {
        var verification = Guid.NewGuid().ToByteArray();

        var verificationKey = new AccountMemberVerificationKey
        {
            Client = client,
            Identity = AuthenticationIdentityIdentity,
            Group = AccountGroup,
            Member = AccountGroupMember,
        };

        using var scope = factory.Services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IAccountMemberVerificationManager>()
            .Set(verificationKey, verification);

        var claims = new Claim[]
        {
            new Claim("badge", "member"),
            new Claim("client", client),
            new Claim("identity", AuthenticationIdentityIdentity),
            new Claim("group", AccountGroup),
            new Claim("member", AccountGroupMember),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, ClaimsJsonSerializerOptions.Options);

        var bearer = WebEncoders.Base64UrlEncode(claimsSerialized);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }

    private static AuthenticationHeaderValue MockValidMemberObserverAuthorizationHeader(this WebApplicationFactory<Program> factory, string client)
    {
        var verification = Guid.NewGuid().ToByteArray();

        var verificationKey = new AccountMemberVerificationKey
        {
            Client = client,
            Identity = AuthenticationIdentityDemo,
            Group = AccountGroup,
            Member = AccountGroupMember,
        };

        using var scope = factory.Services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IAccountMemberVerificationManager>()
            .Set(verificationKey, verification);

        var claims = new Claim[]
        {
            new Claim("badge", "member"),
            new Claim("client", client),
            new Claim("identity", AuthenticationIdentityDemo),
            new Claim("group", AccountGroup),
            new Claim("member", AccountGroupMember),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, ClaimsJsonSerializerOptions.Options);

        var bearer = WebEncoders.Base64UrlEncode(claimsSerialized);

        return new AuthenticationHeaderValue("Bearer", bearer);
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
            new Claim("badge", "identity"),
            new Claim("client", Client),
            new Claim("identity", AuthenticationIdentityAdmin),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, ClaimsJsonSerializerOptions.Options);

        var bearer = WebEncoders.Base64UrlEncode(claimsSerialized);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }

    private static AuthenticationHeaderValue MockInvalidAuthenticationIdentityAuthorizationHeader(this WebApplicationFactory<Program> factory)
    {
        var verification = Guid.NewGuid().ToByteArray();

        var claims = new Claim[]
        {
            new Claim("badge", "identity"),
            new Claim("client", Client),
            new Claim("identity", AuthenticationIdentityIdentity),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, ClaimsJsonSerializerOptions.Options);

        var bearer = WebEncoders.Base64UrlEncode(claimsSerialized);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }

    private static AuthenticationHeaderValue MockInvalidDemoAuthorizationHeader(this WebApplicationFactory<Program> factory)
    {
        var verification = Guid.NewGuid().ToByteArray();

        var claims = new Claim[]
        {
            new Claim("badge", "identity"),
            new Claim("client", Client),
            new Claim("identity", AuthenticationIdentityDemo),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, ClaimsJsonSerializerOptions.Options);

        var bearer = WebEncoders.Base64UrlEncode(claimsSerialized);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }

    private static AuthenticationHeaderValue MockInvalidChiefAuthorizationHeader(this WebApplicationFactory<Program> factory)
    {
        var verification = Guid.NewGuid().ToByteArray();

        var claims = new Claim[]
        {
            new Claim("badge", "member"),
            new Claim("client", Client),
            new Claim("identity", AuthenticationIdentityIdentity),
            new Claim("group", AccountGroup),
            new Claim("member", AccountGroupChief),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, ClaimsJsonSerializerOptions.Options);

        var bearer = WebEncoders.Base64UrlEncode(claimsSerialized);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }

    private static AuthenticationHeaderValue MockInvalidChiefObserverAuthorizationHeader(this WebApplicationFactory<Program> factory)
    {
        var verification = Guid.NewGuid().ToByteArray();

        var claims = new Claim[]
        {
            new Claim("badge", "member"),
            new Claim("client", Client),
            new Claim("identity", AuthenticationIdentityDemo),
            new Claim("group", AccountGroup),
            new Claim("member", AccountGroupChief),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, ClaimsJsonSerializerOptions.Options);

        var bearer = WebEncoders.Base64UrlEncode(claimsSerialized);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }

    private static AuthenticationHeaderValue MockInvalidMemberAuthorizationHeader(this WebApplicationFactory<Program> factory)
    {
        var verification = Guid.NewGuid().ToByteArray();

        var claims = new Claim[]
        {
            new Claim("badge", "member"),
            new Claim("client", Client),
            new Claim("identity", AuthenticationIdentityIdentity),
            new Claim("group", AccountGroup),
            new Claim("member", AccountGroupMember),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, ClaimsJsonSerializerOptions.Options);

        var bearer = WebEncoders.Base64UrlEncode(claimsSerialized);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }

    private static AuthenticationHeaderValue MockInvalidMemberObserverAuthorizationHeader(this WebApplicationFactory<Program> factory)
    {
        var verification = Guid.NewGuid().ToByteArray();

        var claims = new Claim[]
        {
            new Claim("badge", "member"),
            new Claim("client", Client),
            new Claim("identity", AuthenticationIdentityDemo),
            new Claim("group", AccountGroup),
            new Claim("member", AccountGroupMember),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, ClaimsJsonSerializerOptions.Options);

        var bearer = WebEncoders.Base64UrlEncode(claimsSerialized);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }

}