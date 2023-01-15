using ChristianSchulz.MultitenancyMonolith.Aggregates.Authentication;
using ChristianSchulz.MultitenancyMonolith.Application.Administration;
using ChristianSchulz.MultitenancyMonolith.Application.Authentication;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace ChristianSchulz.MultitenancyMonolith.Server.EndpointTests;

internal static class TestConfiguration
{
    public const string ClientName = "endpoint-tests";

    public const string AdminIdentity = "admin";
    public const string AdminMailAddress = "admin@localhost";
    public const string AdminSecret = "secret";

    public const string ChiefIdentity = "chief";
    public const string chiefMailAddress = "chief@localhost";
    public const string chiefSecret = "secret";

    public const string DefaultIdentity = "secure";
    public const string DefaultMailAddress = "default@localhost";
    public const string DefaultSecret = "secret";

    public const string GuestIdentity = "demo";
    public const string GuestMailAddress = "demo@localhost";
    public const string GuestSecret = "secret";

    public const string Group1 = "group-1";
    public const string Group1chief = "chief-group-1";
    public const string Group1Member = "member-group-1";

    public const string Group2 = "group-2";
    public const string Group2chief = "chief-group-2";
    public const string Group2Member = "member-group-2";

    private static readonly IDictionary<string,string> _configuration = new Dictionary<string, string>()
    {
        { "SeedData:Authentication:Identities:0:UniqueName", AdminIdentity },
        { "SeedData:Authentication:Identities:0:MailAddress", AdminMailAddress },
        { "SeedData:Authentication:Identities:0:Secret", AdminSecret },
        { "SeedData:Authentication:Identities:1:UniqueName", DefaultIdentity },
        { "SeedData:Authentication:Identities:1:MailAddress", DefaultMailAddress },
        { "SeedData:Authentication:Identities:1:Secret", DefaultSecret },
        { "SeedData:Authentication:Identities:2:UniqueName", ChiefIdentity },
        { "SeedData:Authentication:Identities:2:MailAddress", chiefMailAddress },
        { "SeedData:Authentication:Identities:2:Secret", chiefSecret },
        { "SeedData:Authentication:Identities:3:UniqueName", GuestIdentity },
        { "SeedData:Authentication:Identities:3:MailAddress", GuestMailAddress },
        { "SeedData:Authentication:Identities:3:Secret", GuestSecret },
        { $"SeedData:Administration:Memberships:0:Group", Group1 },
        { $"SeedData:Administration:Memberships:0:Member", Group1chief },
        { $"SeedData:Administration:Memberships:0:Identity", ChiefIdentity },
        { $"SeedData:Administration:Memberships:1:Group", Group1 },
        { $"SeedData:Administration:Memberships:1:Member", Group1Member },
        { $"SeedData:Administration:Memberships:1:Identity", DefaultIdentity },
        { $"SeedData:Administration:Memberships:2:Group", Group1 },
        { $"SeedData:Administration:Memberships:2:Member", Group1Member },
        { $"SeedData:Administration:Memberships:2:Identity", GuestIdentity },
        { $"SeedData:Administration:Memberships:3:Group", Group2 },
        { $"SeedData:Administration:Memberships:3:Member", Group2chief },
        { $"SeedData:Administration:Memberships:3:Identity", ChiefIdentity },
        { $"SeedData:Administration:Memberships:4:Group", Group2 },
        { $"SeedData:Administration:Memberships:4:Member", Group2Member },
        { $"SeedData:Administration:Memberships:4:Identity", DefaultIdentity },
        { $"SeedData:Administration:Memberships:5:Group", Group2 },
        { $"SeedData:Administration:Memberships:5:Member", Group2Member },
        { $"SeedData:Administration:Memberships:5:Identity", GuestIdentity },
        { $"SeedData:Administration:Members:{Group1}:0:UniqueName", Group1chief },
        { $"SeedData:Administration:Members:{Group1}:1:UniqueName", Group1Member },
        { $"SeedData:Administration:Members:{Group2}:0:UniqueName", Group2chief },
        { $"SeedData:Administration:Members:{Group2}:1:UniqueName", Group2Member }
    };

    public static WebApplicationFactory<Program> WithInMemoryData(this WebApplicationFactory<Program> factory)
        => factory.WithWebHostBuilder(app => app
            .ConfigureAppConfiguration((_, config) =>
            {
                config.Sources.Clear();
                config.AddInMemoryCollection(_configuration!);
            }));


    public static AuthenticationHeaderValue MockValidIdentityAuthorizationHeader(this WebApplicationFactory<Program> factory, string identity)
        => factory.MockValidIdentityAuthorizationHeader(claimName =>
            claimName switch
            {
                "identity" => identity,
                "client" => ClientName,
                _ => throw new UnreachableException("Claim `{claimName}` is not supported.")
            });

    public static AuthenticationHeaderValue MockValidIdentityAuthorizationHeader(this WebApplicationFactory<Program> factory, Func<string, string> claimValueFactory)
    {
        var verfication = MockIdentityVerfication(factory, claimValueFactory);
        var bearer = CreateIdentityBearer(verfication, claimValueFactory);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }

    public static AuthenticationHeaderValue MockValidMemberAuthorizationHeader(this WebApplicationFactory<Program> factory, string identity, string group, string member)
        => factory.MockValidMemberAuthorizationHeader(claimName =>
            claimName switch
            {
                "client" => ClientName,
                "identity" => identity,
                "group" => group,
                "member" => member,
                _ => throw new UnreachableException("Claim `{claimName}` is not supported.")
            });


    public static AuthenticationHeaderValue MockValidMemberAuthorizationHeader(this WebApplicationFactory<Program> factory, Func<string, string> claimValueFactory)
    {
        var verfication = MockMemberVerfication(factory, claimValueFactory);
        var bearer = CreateMemberBearer(verfication, claimValueFactory);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }

    public static AuthenticationHeaderValue MockInvalidIdentityAuthorizationHeader(this WebApplicationFactory<Program> factory, string identity)
        => factory.MockInvalidIdentityAuthorizationHeader(claimName =>
            claimName switch
            {
                "identity" => identity,
                "client" => ClientName,
                _ => throw new UnreachableException("Claim `{claimName}` is not supported.")
            });

    public static AuthenticationHeaderValue MockInvalidIdentityAuthorizationHeader(this WebApplicationFactory<Program> _, Func<string, string> claimValueFactory)
    {
        var verfication = MockInvalidVerfication();
        var bearer = CreateIdentityBearer(verfication, claimValueFactory);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }

    public static AuthenticationHeaderValue MockInvalidMemberAuthorizationHeader(this WebApplicationFactory<Program> factory, string identity, string group, string member)
        => factory.MockInvalidMemberAuthorizationHeader(claimName =>
            claimName switch
            {
                "client" => ClientName,
                "identity" => identity,
                "group" => group,
                "member" => member,
                _ => throw new UnreachableException("Claim `{claimName}` is not supported.")
            });

    public static AuthenticationHeaderValue MockInvalidMemberAuthorizationHeader(this WebApplicationFactory<Program> _, Func<string, string> claimValueFactory)
    {
        var verfication = MockInvalidVerfication();
        var bearer = CreateMemberBearer(verfication, claimValueFactory);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }

    private static string MockInvalidVerfication()
    {
        var verfication = Guid.NewGuid().ToByteArray();

        return Convert.ToBase64String(verfication);
    }

    private static string MockIdentityVerfication(WebApplicationFactory<Program> factory, Func<string, string> claimValueFactory)
    {
        var verfication = Guid.NewGuid().ToByteArray();

        var verficationKey = new IdentityVerficationKey
        {
            Client = claimValueFactory.Invoke("client"),
            Identity = claimValueFactory.Invoke("identity"),
        };

        using var scope = factory.Services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IIdentityVerficationManager>()
            .Set(verficationKey, verfication);

        return Convert.ToBase64String(verfication);
    }

    private static string MockMemberVerfication(WebApplicationFactory<Program> factory, Func<string, string> claimValueFactory)
    {
        var verfication = Guid.NewGuid().ToByteArray();

        var verficationKey = new MembershipVerficationKey
        {
            Client = claimValueFactory.Invoke("client"),
            Group = claimValueFactory.Invoke("group"),
            Member = claimValueFactory.Invoke("member"),
        };

        using var scope = factory.Services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IMembershipVerficationManager>()
            .Set(verficationKey, verfication);

        return Convert.ToBase64String(verfication);
    }

    private static string CreateIdentityBearer(string verification, Func<string, string> claimValueFactory)
    {
        var claims = new Claim[]
        {
            new Claim("client",  claimValueFactory.Invoke("client")),
            new Claim("identity", claimValueFactory.Invoke("identity")),
            new Claim("verification", verification, ClaimValueTypes.Base64Binary)
        };

        var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, ClaimsJsonSerializerOptions.Options);

        return WebEncoders.Base64UrlEncode(claimsSerialized);
    }

    private static string CreateMemberBearer(string verification, Func<string, string> claimValueFactory)
    {
        var claims = new Claim[]
        {
            new Claim("client", claimValueFactory.Invoke("client")),
            new Claim("identity", claimValueFactory.Invoke("identity")),
            new Claim("group", claimValueFactory.Invoke("group")),
            new Claim("member", claimValueFactory.Invoke("member")),
            new Claim("verification", verification, ClaimValueTypes.Base64Binary)
        };

        var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, ClaimsJsonSerializerOptions.Options);

        return WebEncoders.Base64UrlEncode(claimsSerialized);
    }

}
