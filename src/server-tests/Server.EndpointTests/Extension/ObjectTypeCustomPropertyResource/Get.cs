using ChristianSchulz.MultitenancyMonolith.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Xunit;
using System.Threading.Tasks;
using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using ChristianSchulz.MultitenancyMonolith.Server;
using ChristianSchulz.MultitenancyMonolith.Objects.Extension;

namespace Extension.ObjectTypeCustomPropertyResource;

public sealed class Get : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Get(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Get_ShouldSucceed_WhenExists()
    {
        // Arrange
        var existingObjectTypeCustomProperty = new ObjectTypeCustomProperty
        {
            UniqueName = $"existing-object-type-custom-property--{Guid.NewGuid()}",
            DisplayName = "Foo Bar",
            PropertyName = "fooBar",
            PropertyType = "string"
        };

        var existingObjectType = new ObjectType
        {
            UniqueName = "business-object",
            CustomProperties = new List<ObjectTypeCustomProperty>
        {
            existingObjectTypeCustomProperty
        }
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/extension/object-types/{existingObjectType.UniqueName}/custom-properties/{existingObjectTypeCustomProperty.UniqueName}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(content);
        Assert.Collection(content.OrderBy(x => x.Key),
            x => Assert.Equal(("displayName", existingObjectTypeCustomProperty.DisplayName), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("objectType", existingObjectType.UniqueName), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("propertyName", existingObjectTypeCustomProperty.PropertyName), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("propertyType", existingObjectTypeCustomProperty.PropertyType), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("uniqueName", existingObjectTypeCustomProperty.UniqueName), (x.Key, (string?)x.Value)));
    }

    [Fact]
    public async Task Get_ShouldFail_WhenInvalidObjectType()
    {
        // Arrange
        var invalidObjectType = "Invalid-object-type";
        var validObjectTypeCustomProperty = "valid-object-type-custom-property";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/extension/object-types/{invalidObjectType}/custom-properties/{validObjectTypeCustomProperty}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Get_ShouldFail_WhenInvalidCustomProperty()
    {
        // Arrange
        var existingObjectType = new ObjectType
        {
            UniqueName = "business-object"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);
        }

        var invalidObjectTypeCustomProperty = "Invalid";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/extension/object-types/{existingObjectType.UniqueName}/custom-properties/{invalidObjectTypeCustomProperty}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Get_ShouldFail_WhenAbsent()
    {
        // Arrange
        var existingObjectType = new ObjectType
        {
            UniqueName = "business-object",
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);
        }

        var absentObjectTypeCustomProperty = "absent-object-type-custom-property";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/extension/object-types/{existingObjectType.UniqueName}/custom-properties/{absentObjectTypeCustomProperty}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}