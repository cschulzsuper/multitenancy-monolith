using ChristianSchulz.MultitenancyMonolith.Backend.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;

namespace Diagnostic.BuildInfo;

public sealed class Get : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    private static readonly IDictionary<string, string> _buildInfoConfiguration = new Dictionary<string, string>()
    {
        {"BuildInfo:BuildNumber", "build-number"},
        {"BuildInfo:BranchName", "branch-name"},
        {"BuildInfo:CommitHash", "commit-hash"},
        {"BuildInfo:ShortCommitHash", "short-commit-hash"},    
    };

    public Get(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Get_ShouldSucceed_WhenExists()
    {
        // Arrange
        using var factory = _factory.WithWebHostBuilder(app => app
            .ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(_buildInfoConfiguration!);
            }));

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/a1/diagnostic/build-info");

        var client = factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(content);
        Assert.Collection(content.OrderBy(x => x.Key),
            x => Assert.Equal(("branchName", "branch-name"), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("buildNumber", "build-number"), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("commitHash", "commit-hash"), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("shortCommitHash", "short-commit-hash"), (x.Key, (string?)x.Value)));
    }
}
