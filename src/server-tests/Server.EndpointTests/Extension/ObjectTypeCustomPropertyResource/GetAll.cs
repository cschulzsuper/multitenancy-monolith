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

namespace Extension.ObjectTypeCustomPropertyResource;

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
        var existingObjectTypeCustomProperty1 = new ObjectTypeCustomProperty
        {
            UniqueName = $"existing-object-type-custom-property-1-{Guid.NewGuid()}",
            DisplayName = "Foo Bar",
            PropertyName = "fooBar1",
            PropertyType = "string"
        };

        var existingObjectTypeCustomProperty2 = new ObjectTypeCustomProperty
        {
            UniqueName = $"existing-object-type-custom-property-2-{Guid.NewGuid()}",
            DisplayName = "Foo Bar",
            PropertyName = "fooBar2",
            PropertyType = "string"
        };

        var existingObjectType = new ObjectType
        {
            UniqueName = "business-object",
            CustomProperties = new List<ObjectTypeCustomProperty>
            {
                existingObjectTypeCustomProperty1, existingObjectTypeCustomProperty2
            }
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/extension/object-types/{existingObjectType.UniqueName}/custom-properties");
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
                Assert.Equal(existingObjectTypeCustomProperty1.DisplayName, x.GetString("displayName"));
                Assert.Equal(existingObjectType.UniqueName, x.GetString("objectType"));
                Assert.Equal(existingObjectTypeCustomProperty1.PropertyName, x.GetString("propertyName"));
                Assert.Equal(existingObjectTypeCustomProperty1.PropertyType, x.GetString("propertyType"));
                Assert.Equal(existingObjectTypeCustomProperty1.UniqueName, x.GetString("uniqueName"));
            },
            x =>
            {
                Assert.Equal(existingObjectTypeCustomProperty2.DisplayName, x.GetString("displayName"));
                Assert.Equal(existingObjectType.UniqueName, x.GetString("objectType"));
                Assert.Equal(existingObjectTypeCustomProperty2.PropertyName, x.GetString("propertyName"));
                Assert.Equal(existingObjectTypeCustomProperty2.PropertyType, x.GetString("propertyType"));
                Assert.Equal(existingObjectTypeCustomProperty2.UniqueName, x.GetString("uniqueName"));
            });
    }

    [Fact]
    public async Task GetAll_ShouldFail_WhenInvalidObjectType()
    {
        // Arrange
        var invalidObjectType = "Invalid-object-type";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/extension/object-types/{invalidObjectType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}