using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Extension;
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

namespace Extension.DistinctionTypeResource;

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
        var existingDistinctionType1 = new DistinctionType
        {
            UniqueName = $"existing-distinction-type-1-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type 1"
        };

        var existingDistinctionType2 = new DistinctionType
        {
            UniqueName = $"existing-distinction-type-2-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type 2"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType1, existingDistinctionType2);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/extension/distinction-types");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

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
                Assert.Equal(existingDistinctionType1.UniqueName, x.GetString("uniqueName"));
                Assert.Equal(existingDistinctionType1.ObjectType, x.GetString("objectType"));
                Assert.Equal(existingDistinctionType1.DisplayName, x.GetString("displayName"));
            },
            x =>
            {
                Assert.Equal(existingDistinctionType2.UniqueName, x.GetString("uniqueName"));
                Assert.Equal(existingDistinctionType2.ObjectType, x.GetString("objectType"));
                Assert.Equal(existingDistinctionType2.DisplayName, x.GetString("displayName"));
            });
    }
}