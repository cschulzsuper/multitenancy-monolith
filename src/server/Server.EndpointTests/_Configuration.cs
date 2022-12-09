using ChristianSchulz.MultitenancyMonolith.Application.Administration;
using ChristianSchulz.MultitenancyMonolith.Application.Authentication;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Core;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace ChristianSchulz.MultitenancyMonolith.Server.EndpointTests;

internal static class TestConfiguration
{
    public const string AdminIdentity = "admin";
    public const string AdminMailAddress = "default@localhost";
    public const string AdminSecret = "default";
    public const string GuestIdentity = "guest";
    public const string GuestMailAddress = "default@localhost";
    public const string GuestSecret = "default";

    public const string DefaultGroup1 = "default-1";
    public const string DefaultGroup1Admin = "default-1-admin";
    public const string DefaultGroup1Guest = "default-1-guest";

    public const string DefaultGroup2 = "default-2";
    public const string DefaultGroup2Admin = "default-2-admin";
    public const string DefaultGroup2Guest = "default-2-guest";

    private static readonly IDictionary<string,string> _configuration = new Dictionary<string, string>()
    {
        { "SeedData:Authentication:Identities:0:UniqueName", AdminIdentity },
        { "SeedData:Authentication:Identities:0:MailAddress", AdminIdentity },
        { "SeedData:Authentication:Identities:0:Secret", AdminSecret },
        { "SeedData:Authentication:Identities:1:UniqueName", GuestIdentity },
        { "SeedData:Authentication:Identities:1:MailAddress", GuestIdentity },
        { "SeedData:Authentication:Identities:1:Secret", GuestSecret },
        { $"SeedData:Administration:Memberships:0:Group", DefaultGroup1 },
        { $"SeedData:Administration:Memberships:0:Member", DefaultGroup1Admin },
        { $"SeedData:Administration:Memberships:0:Identity", AdminIdentity },
        { $"SeedData:Administration:Memberships:1:Group", DefaultGroup1 },
        { $"SeedData:Administration:Memberships:1:Member", DefaultGroup1Guest },
        { $"SeedData:Administration:Memberships:1:Identity", GuestIdentity },
        { $"SeedData:Administration:Memberships:2:Group", DefaultGroup2 },
        { $"SeedData:Administration:Memberships:2:Member", DefaultGroup2Admin },
        { $"SeedData:Administration:Memberships:2:Identity", AdminIdentity },
        { $"SeedData:Administration:Memberships:3:Group", DefaultGroup2 },
        { $"SeedData:Administration:Memberships:3:Member", DefaultGroup2Guest },
        { $"SeedData:Administration:Memberships:3:Identity", GuestIdentity },
        { $"SeedData:Administration:Members:{DefaultGroup1}:0:UniqueName", DefaultGroup1Admin },
        { $"SeedData:Administration:Members:{DefaultGroup1}:1:UniqueName", DefaultGroup1Guest },
        { $"SeedData:Administration:Members:{DefaultGroup2}:0:UniqueName", DefaultGroup2Admin },
        { $"SeedData:Administration:Members:{DefaultGroup2}:1:UniqueName", DefaultGroup2Guest },
    };

    public static WebApplicationFactory<Program> WithInMemoryData(this WebApplicationFactory<Program> factory)
        => factory.WithWebHostBuilder(
            app => app.ConfigureAppConfiguration((_, config) =>
            {
                config.Sources.Clear();
                config.AddInMemoryCollection(_configuration!);
            }));

    public static AuthenticationHeaderValue MockValidIdentityAuthorizationHeader(this WebApplicationFactory<Program> factory, string identity)
    {
        var verfication = MockIdentityVerfication(factory, identity);
        var bearer = CreateIdentityBearer(identity, verfication);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }

    public static AuthenticationHeaderValue MockValidMemberAuthorizationHeader(this WebApplicationFactory<Program> factory, string identity, string group, string member)
    {
        var verfication = MockMemberVerfication(factory, group, member);
        var bearer = CreateMemberBearer(identity, verfication, group, member);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }

    public static AuthenticationHeaderValue MockInvalidIdentityAuthorizationHeader(this WebApplicationFactory<Program> _, string identity)
    {
        var verfication = MockInvalidVerfication();
        var bearer = CreateIdentityBearer(identity, verfication);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }

    public static AuthenticationHeaderValue MockInvalidMemberAuthorizationHeader(this WebApplicationFactory<Program> _, string identity, string group, string member)
    {
        var verfication = MockInvalidVerfication();
        var bearer = CreateMemberBearer(identity, verfication, group, member);

        return new AuthenticationHeaderValue("Bearer", bearer);
    }

    private static string MockInvalidVerfication()
    {
        var verfication = Guid.NewGuid().ToByteArray();

        return Convert.ToBase64String(verfication);
    }

    private static string MockIdentityVerfication(WebApplicationFactory<Program> factory, string identity)
    {
        var verfication = Guid.NewGuid().ToByteArray();

        using var scope = factory.Services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IIdentityVerficationManager>()
            .Set(identity, verfication);

        return Convert.ToBase64String(verfication);
    }

    private static string MockMemberVerfication(WebApplicationFactory<Program> factory, string group, string member)
    {
        var verfication = Guid.NewGuid().ToByteArray();

        using var scope = factory.Services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IMembershipVerficationManager>()
            .Set(group, member, verfication);

        return Convert.ToBase64String(verfication);
    }

    private static string CreateIdentityBearer(string identity, string verification)
    {
        var claims = new Claim[]
        {
            new Claim("Identity", identity),
            new Claim("Verification", verification, ClaimValueTypes.Base64Binary)
        };

        var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, ClaimsJsonSerializerOptions.Options);

        return WebEncoders.Base64UrlEncode(claimsSerialized);
    }

    private static string CreateMemberBearer(string identity, string verification, string group, string member)
    {
        var claims = new Claim[]
        {
            new Claim("Identity", identity),
            new Claim("Group", group),
            new Claim("Member", member),
            new Claim("Verification", verification, ClaimValueTypes.Base64Binary)
        };

        var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, ClaimsJsonSerializerOptions.Options);

        return WebEncoders.Base64UrlEncode(claimsSerialized);
    }

}
