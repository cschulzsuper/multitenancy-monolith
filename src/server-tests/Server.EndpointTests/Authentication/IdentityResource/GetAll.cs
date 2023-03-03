using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Authentication;
using ChristianSchulz.MultitenancyMonolith.Server;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Authentication.IdentityResource;

public sealed class GetAll : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public GetAll(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task GetAll_ShouldSucceed()
    {
        // Arrange
        var existingIdentity1 = new Identity
        {
            UniqueName = $"existing-identity-1-{Guid.NewGuid()}",
            MailAddress = "info@localhost",
            Secret = "foo-bar"
        };

        var existingIdentity2 = new Identity
        {
            UniqueName = $"existing-identity-2-{Guid.NewGuid()}",
            MailAddress = "info@localhost",
            Secret = "foo-bar"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<Identity>>()
                .Insert(existingIdentity1, existingIdentity2);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/authentication/identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonDocument>();
        Assert.NotNull(content);
        Assert.Collection(content.RootElement.EnumerateArray().OrderBy(x => x.GetString("uniqueName")),
            x =>
            {
                Assert.Equal(existingIdentity1.MailAddress, x.GetString("mailAddress"));
                Assert.Equal(existingIdentity1.UniqueName, x.GetString("uniqueName"));
            },
            x =>
            {
                Assert.Equal(existingIdentity2.MailAddress, x.GetString("mailAddress"));
                Assert.Equal(existingIdentity2.UniqueName, x.GetString("uniqueName"));
            });
    }
}