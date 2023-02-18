using ChristianSchulz.MultitenancyMonolith.Application.Authentication;
using ChristianSchulz.MultitenancyMonolith.Application.Authorization;
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

    public const string IdentityAdmin = "admin";
    public const string IdentityAdminMailAddress = "admin@localhost";
    public const string IdentityAdminSecret = "secret";

    public const string IdentityIdentity = "identity";
    public const string IdentityIdentityMailAddress = "identity@localhost";
    public const string IdentityIdentitySecret = "secret";

    public const string IdentityDemo = "demo";
    public const string IdentityDemoMailAddress = "demo@localhost";
    public const string IdentityDemoSecret = "secret";

    public const string Group = "group";
    
    public const string GroupChief = "chief-member";
    public const string GroupChiefMailAddress = "chief@localhost";

    public const string GroupMember = "member";
    public const string GroupMemberMailAddress = "member@localhost";

    private static readonly IDictionary<string, string> _configuration = new Dictionary<string, string>()
    {
        {"AllowedClients:0:UniqueName", "swagger"},
        {"AllowedClients:0:Scopes:0", "swagger-json"},
        {"AllowedClients:0:Scopes:1", "endpoints"},
        {"AllowedClients:1:UniqueName", "endpoint-tests"},
        {"AllowedClients:1:Scopes:1", "endpoints"},
        {"AllowedClients:2:UniqueName", "security-tests"},
        {"AllowedClients:2:Scopes:1", "endpoints"},

        {"SeedData:Authentication:Identities:0:UniqueName", IdentityAdmin},
        {"SeedData:Authentication:Identities:0:MailAddress", IdentityAdminMailAddress},
        {"SeedData:Authentication:Identities:0:Secret", IdentityAdminSecret},
        {"SeedData:Authentication:Identities:1:UniqueName", IdentityIdentity},
        {"SeedData:Authentication:Identities:1:MailAddress", IdentityIdentityMailAddress},
        {"SeedData:Authentication:Identities:1:Secret", IdentityIdentitySecret},
        {"SeedData:Authentication:Identities:2:UniqueName", IdentityDemo},
        {"SeedData:Authentication:Identities:2:MailAddress", IdentityDemoMailAddress},
        {"SeedData:Authentication:Identities:2:Secret", IdentityDemoSecret},
        {$"SeedData:Administration:Members:{Group}:0:UniqueName", GroupChief},
        {$"SeedData:Administration:Members:{Group}:0:MailAddress", GroupChiefMailAddress},
        {$"SeedData:Administration:Members:{Group}:0:Identities:0:UniqueName", IdentityIdentity},
        {$"SeedData:Administration:Members:{Group}:0:Identities:1:UniqueName", IdentityDemo},
        {$"SeedData:Administration:Members:{Group}:1:UniqueName", GroupMember},
        {$"SeedData:Administration:Members:{Group}:1:MailAddress", GroupMemberMailAddress},
        {$"SeedData:Administration:Members:{Group}:1:Identities:0:UniqueName", IdentityIdentity},
        {$"SeedData:Administration:Members:{Group}:1:Identities:1:UniqueName", IdentityDemo},
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
            Identity = IdentityAdmin
        };

        using var scope = factory.Services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IIdentityVerificationManager>()
            .Set(verificationKey, verification);

        var claims = new Claim[]
        {
            new Claim("badge", "identity"),
            new Claim("client", client),
            new Claim("identity", IdentityAdmin),
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
            Identity = IdentityIdentity
        };

        using var scope = factory.Services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IIdentityVerificationManager>()
            .Set(verificationKey, verification);

        var claims = new Claim[]
        {
            new Claim("badge", "identity"),
            new Claim("client", client),
            new Claim("identity", IdentityIdentity),
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
            Identity = IdentityDemo
        };

        using var scope = factory.Services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IIdentityVerificationManager>()
            .Set(verificationKey, verification);

        var claims = new Claim[]
        {
            new Claim("badge", "identity"),
            new Claim("client", client),
            new Claim("identity", IdentityDemo),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, ClaimsJsonSerializerOptions.Options);

        var bearer = WebEncoders.Base64UrlEncode(claimsSerialized);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }

    private static AuthenticationHeaderValue MockValidChiefAuthorizationHeader(this WebApplicationFactory<Program> factory, string client)
    {
        var verification = Guid.NewGuid().ToByteArray();

        var verificationKey = new MemberVerificationKey
        {
            Client = client,
            Identity = IdentityIdentity,
            Group = Group,
            Member = GroupChief,
        };

        using var scope = factory.Services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IMemberVerificationManager>()
            .Set(verificationKey, verification);

        var claims = new Claim[]
        {
            new Claim("badge", "member"),
            new Claim("client", client),
            new Claim("identity", IdentityIdentity),
            new Claim("group", Group),
            new Claim("member", GroupChief),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, ClaimsJsonSerializerOptions.Options);

        var bearer = WebEncoders.Base64UrlEncode(claimsSerialized);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }

    private static AuthenticationHeaderValue MockValidChiefObserverAuthorizationHeader(this WebApplicationFactory<Program> factory, string client)
    {
        var verification = Guid.NewGuid().ToByteArray();

        var verificationKey = new MemberVerificationKey
        {
            Client = client,
            Identity = IdentityDemo,
            Group = Group,
            Member = GroupChief,
        };

        using var scope = factory.Services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IMemberVerificationManager>()
            .Set(verificationKey, verification);

        var claims = new Claim[]
        {
            new Claim("badge", "member"),
            new Claim("client", client),
            new Claim("identity", IdentityDemo),
            new Claim("group", Group),
            new Claim("member", GroupChief),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, ClaimsJsonSerializerOptions.Options);

        var bearer = WebEncoders.Base64UrlEncode(claimsSerialized);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }

    private static AuthenticationHeaderValue MockValidMemberAuthorizationHeader(this WebApplicationFactory<Program> factory, string client)
    {
        var verification = Guid.NewGuid().ToByteArray();

        var verificationKey = new MemberVerificationKey
        {
            Client = client,
            Identity = IdentityIdentity,
            Group = Group,
            Member = GroupMember,
        };

        using var scope = factory.Services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IMemberVerificationManager>()
            .Set(verificationKey, verification);

        var claims = new Claim[]
        {
            new Claim("badge", "member"),
            new Claim("client", client),
            new Claim("identity", IdentityIdentity),
            new Claim("group", Group),
            new Claim("member", GroupMember),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, ClaimsJsonSerializerOptions.Options);

        var bearer = WebEncoders.Base64UrlEncode(claimsSerialized);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }

    private static AuthenticationHeaderValue MockValidMemberObserverAuthorizationHeader(this WebApplicationFactory<Program> factory, string client)
    {
        var verification = Guid.NewGuid().ToByteArray();

        var verificationKey = new MemberVerificationKey
        {
            Client = client,
            Identity = IdentityDemo,
            Group = Group,
            Member = GroupMember,
        };

        using var scope = factory.Services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IMemberVerificationManager>()
            .Set(verificationKey, verification);

        var claims = new Claim[]
        {
            new Claim("badge", "member"),
            new Claim("client", client),
            new Claim("identity", IdentityDemo),
            new Claim("group", Group),
            new Claim("member", GroupMember),
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
            MockIdentity => factory.MockInvalidIdentityAuthorizationHeader(),
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
            new Claim("identity", IdentityAdmin),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, ClaimsJsonSerializerOptions.Options);

        var bearer = WebEncoders.Base64UrlEncode(claimsSerialized);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }

    private static AuthenticationHeaderValue MockInvalidIdentityAuthorizationHeader(this WebApplicationFactory<Program> factory)
    {
        var verification = Guid.NewGuid().ToByteArray();

        var claims = new Claim[]
        {
            new Claim("badge", "identity"),
            new Claim("client", Client),
            new Claim("identity", IdentityIdentity),
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
            new Claim("identity", IdentityDemo),
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
            new Claim("identity", IdentityIdentity),
            new Claim("group", Group),
            new Claim("member", GroupChief),
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
            new Claim("identity", IdentityDemo),
            new Claim("group", Group),
            new Claim("member", GroupChief),
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
            new Claim("identity", IdentityIdentity),
            new Claim("group", Group),
            new Claim("member", GroupMember),
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
            new Claim("identity", IdentityDemo),
            new Claim("group", Group),
            new Claim("member", GroupMember),
            new Claim("verification", Convert.ToBase64String(verification)),
        };

        var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, ClaimsJsonSerializerOptions.Options);

        var bearer = WebEncoders.Base64UrlEncode(claimsSerialized);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }

}