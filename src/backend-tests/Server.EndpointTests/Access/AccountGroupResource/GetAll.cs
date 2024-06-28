using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Access.AccountGroupResource;

public sealed class GetAll 
{
    [Fact]
    public async Task GetAll_ShouldSucceed()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingAccountGroup1 = new AccountGroup
        {
            UniqueName = $"existing-account-group-1-{Guid.NewGuid()}"
        };

        var existingAccountGroup2 = new AccountGroup
        {
            UniqueName = $"existing-account-group-2-{Guid.NewGuid()}"
        };

        using (var scope = application.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountGroup>>()
                .Insert(existingAccountGroup1, existingAccountGroup2);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/a1/access/account-groups");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonDocument>();
        Assert.NotNull(content);
        Assert.Collection(content.RootElement.EnumerateArray().OrderBy(x => x.GetString("uniqueName")),
            x =>
            {
                Assert.Equal(existingAccountGroup1.UniqueName, x.GetString("uniqueName"));
            },
            x =>
            {
                Assert.Equal(existingAccountGroup2.UniqueName, x.GetString("uniqueName"));
            });
    }
}