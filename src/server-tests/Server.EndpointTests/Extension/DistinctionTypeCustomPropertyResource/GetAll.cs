using ChristianSchulz.MultitenancyMonolith.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using System;
using System.Linq;
using ChristianSchulz.MultitenancyMonolith.Server;
using ChristianSchulz.MultitenancyMonolith.Objects.Extension;

namespace Extension.DistinctionTypeCustomPropertyResource;

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
        var existingDistinctionTypeCustomProperty1 = new DistinctionTypeCustomProperty
        {
            UniqueName = $"existing-distinction-type-custom-property-1-{Guid.NewGuid()}",
        };

        var existingDistinctionTypeCustomProperty2 = new DistinctionTypeCustomProperty
        {
            UniqueName = $"existing-distinction-type-custom-property-2-{Guid.NewGuid()}",
        };

        var existingDistinctionType = new DistinctionType
        {
            UniqueName = $"existing-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type",
            CustomProperties = new List<DistinctionTypeCustomProperty>
            {
                existingDistinctionTypeCustomProperty1, existingDistinctionTypeCustomProperty2
            }
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/extension/distinction-types/{existingDistinctionType.UniqueName}/custom-properties");
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
                Assert.Equal(existingDistinctionTypeCustomProperty1.UniqueName, x.GetString("uniqueName"));
                Assert.Equal(existingDistinctionType.UniqueName, x.GetString("distinctionType"));
            },
            x =>
            {
                Assert.Equal(existingDistinctionTypeCustomProperty2.UniqueName, x.GetString("uniqueName"));
                Assert.Equal(existingDistinctionType.UniqueName, x.GetString("distinctionType"));
            });
    }

    [Fact]
    public async Task GetAll_ShouldFail_WhenInvalidDistinctionType()
    {
        // Arrange
        var invalidDistinctionType = "Invalid-distinction-type";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/extension/distinction-types/{invalidDistinctionType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}