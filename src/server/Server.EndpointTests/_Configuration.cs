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
    public const string AdminSecret = "default";
    public const string GuestIdentity = "guest";
    public const string GuestSecret = "default";

    public const string DefaultGroup = "default";
    public const string DefaultGroupAdmin = "default-admin";
    public const string DefaultGroupAdminIdentity = AdminIdentity;
    public const string DefaultGroupGuest = "default-guest";
    public const string DefaultGroupGuestIdentity = GuestIdentity;

    private static readonly IDictionary<string,string> _configuration = new Dictionary<string, string>()
    {
        { "SeedData:Authentication:Identities:0:UniqueName", AdminIdentity },
        { "SeedData:Authentication:Identities:0:Secret", AdminSecret },
        { "SeedData:Authentication:Identities:1:UniqueName", GuestIdentity },
        { "SeedData:Authentication:Identities:1:Secret", GuestSecret },
        { $"SeedData:Administration:Members:{DefaultGroup}:0:UniqueName", DefaultGroupAdmin },
        { $"SeedData:Administration:Members:{DefaultGroup}:0:Identity", DefaultGroupAdminIdentity },
        { $"SeedData:Administration:Members:{DefaultGroup}:1:UniqueName", DefaultGroupGuest },
        { $"SeedData:Administration:Members:{DefaultGroup}:1:Identity", DefaultGroupGuestIdentity }
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
            .GetRequiredService<IMemberVerficationManager>()
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
