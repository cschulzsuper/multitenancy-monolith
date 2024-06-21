using ChristianSchulz.MultitenancyMonolith.Backend.Server;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Extension;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Extension.ObjectTypeCustomPropertyResource;

public sealed class GetAll 
{
    [Fact]
    public async Task GetAll_ShouldSucceed()
    {
        // Arrange
        using var application = MockWebApplication.Create();

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

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/a1/extension/object-types/{existingObjectType.UniqueName}/custom-properties");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

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
        using var application = MockWebApplication.Create();

        var invalidObjectType = "Invalid-object-type";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/a1/extension/object-types/{invalidObjectType}/custom-properties");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}