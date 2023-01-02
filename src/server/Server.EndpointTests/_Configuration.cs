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
    public const string AdminMailAddress = "admin@localhost";
    public const string AdminSecret = "secret";

    public const string ChiefIdentity = "chief";
    public const string ChiefMailAddress = "chief@localhost";
    public const string ChiefSecret = "secret";

    public const string DefaultIdentity = "secure";
    public const string DefaultMailAddress = "default@localhost";
    public const string DefaultSecret = "secret";

    public const string GuestIdentity = "demo";
    public const string GuestMailAddress = "demo@localhost";
    public const string GuestSecret = "secret";

    public const string Group1 = "group-1";
    public const string Group1Chief = "chief-group-1";
    public const string Group1Member = "member-group-1";

    public const string Group2 = "group-2";
    public const string Group2Chief = "chief-group-2";
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
        { "SeedData:Authentication:Identities:2:MailAddress", ChiefMailAddress },
        { "SeedData:Authentication:Identities:2:Secret", ChiefSecret },
        { "SeedData:Authentication:Identities:3:UniqueName", GuestIdentity },
        { "SeedData:Authentication:Identities:3:MailAddress", GuestMailAddress },
        { "SeedData:Authentication:Identities:3:Secret", GuestSecret },
        { $"SeedData:Administration:Memberships:0:Group", Group1 },
        { $"SeedData:Administration:Memberships:0:Member", Group1Chief },
        { $"SeedData:Administration:Memberships:0:Identity", ChiefIdentity },
        { $"SeedData:Administration:Memberships:1:Group", Group1 },
        { $"SeedData:Administration:Memberships:1:Member", Group1Member },
        { $"SeedData:Administration:Memberships:1:Identity", DefaultIdentity },
        { $"SeedData:Administration:Memberships:2:Group", Group1 },
        { $"SeedData:Administration:Memberships:2:Member", Group1Member },
        { $"SeedData:Administration:Memberships:2:Identity", GuestIdentity },
        { $"SeedData:Administration:Memberships:3:Group", Group2 },
        { $"SeedData:Administration:Memberships:3:Member", Group2Chief },
        { $"SeedData:Administration:Memberships:3:Identity", ChiefIdentity },
        { $"SeedData:Administration:Memberships:4:Group", Group2 },
        { $"SeedData:Administration:Memberships:4:Member", Group2Member },
        { $"SeedData:Administration:Memberships:4:Identity", DefaultIdentity },
        { $"SeedData:Administration:Memberships:5:Group", Group2 },
        { $"SeedData:Administration:Memberships:5:Member", Group2Member },
        { $"SeedData:Administration:Memberships:5:Identity", GuestIdentity },
        { $"SeedData:Administration:Members:{Group1}:0:UniqueName", Group1Chief },
        { $"SeedData:Administration:Members:{Group1}:1:UniqueName", Group1Member },
        { $"SeedData:Administration:Members:{Group2}:0:UniqueName", Group2Chief },
        { $"SeedData:Administration:Members:{Group2}:1:UniqueName", Group2Member }
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
