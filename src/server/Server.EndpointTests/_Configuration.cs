using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace ChristianSchulz.MultitenancyMonolith.Server.EndpointTests;

internal static class TestConfiguration
{
    public const string AdminIdentity = "admin";
    public const string AdminSecret = "default";
    public const string GuestIdentity = "guest";
    public const string GuestSecret = "default";

    private static IDictionary<string,string> _configuration = new Dictionary<string, string>()
    {
        { "SeedData:Authentication:Identities:0:UniqueName", "admin" },
        { "SeedData:Authentication:Identities:0:Secret", "default" },
        { "SeedData:Authentication:Identities:1:UniqueName", "guest" },
        { "SeedData:Authentication:Identities:1:Secret", "default" },
        { "SeedData:Administration:Members:default:0:UniqueName", "default-admin" },
        { "SeedData:Administration:Members:default:0:Identity},", "admin" },
        { "SeedData:Administration:Members:default:1:UniqueName", "default-guest" },
        { "SeedData:Administration:Members:default:1:Identity},", "default" }
    };

    public static WebApplicationFactory<Program> WithInMemoryData(this WebApplicationFactory<Program> factory)
        => factory.WithWebHostBuilder(
            app => app.ConfigureAppConfiguration((_, config) =>
            {
                config.Sources.Clear();
                config.AddInMemoryCollection(_configuration!);
            }));
}
