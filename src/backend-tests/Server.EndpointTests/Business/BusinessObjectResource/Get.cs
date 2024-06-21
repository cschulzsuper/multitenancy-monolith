using ChristianSchulz.MultitenancyMonolith.Backend.Server;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Business;
using ChristianSchulz.MultitenancyMonolith.Objects.Extension;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;

namespace Business.BusinessObjectResource;

public sealed class Get 
{
    [Fact]
    public async Task Get_ShouldSucceed_WithoutCustomProperties()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingBusinessObject = new BusinessObject
        {
            UniqueName = $"existing-business-object-{Guid.NewGuid()}"
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<BusinessObject>>()
                .Insert(existingBusinessObject);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/a1/business/business-objects/{existingBusinessObject.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(content);
        Assert.Collection(content.OrderBy(x => x.Key),
            x =>
            {
                Assert.Equal("customProperties", x.Key);
                Assert.NotNull(x.Value);
                Assert.Empty(x.Value.AsObject().ToArray());
            },
            x => Assert.Equal(("uniqueName", existingBusinessObject.UniqueName), (x.Key, (string?)x.Value)));
    }

    [Fact]
    public async Task Get_ShouldSucceed_WithCustomProperties()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingObjectTypeCustomProperty = new ObjectTypeCustomProperty
        {
            UniqueName = $"custom-property-{Guid.NewGuid()}",
            DisplayName = "Custom Property",
            PropertyName = "customProperty",
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

        var existingBusinessObject = new BusinessObject
        {
            UniqueName = $"existing-business-object-{Guid.NewGuid()}",
            CustomProperties = new Dictionary<string, object>
            {
                ["customProperty"] = "custom-property-value"
            }
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);

            scope.ServiceProvider
                .GetRequiredService<IRepository<BusinessObject>>()
                .Insert(existingBusinessObject);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/a1/business/business-objects/{existingBusinessObject.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(content);
        Assert.Collection(content.OrderBy(x => x.Key),
            x =>
            {
                Assert.Equal("customProperties", x.Key);
                Assert.NotNull(x.Value);
                Assert.Collection(x.Value.AsObject().ToDictionary(z => z.Key, z => z.Value),
                    y => { Assert.Equal(("customProperty", (string?)existingBusinessObject["customProperty"]), (y.Key, y.Value?.GetValue<string>())); });
            },
            x => Assert.Equal(("uniqueName", existingBusinessObject.UniqueName), (x.Key, (string?)x.Value)));
    }

    [Fact]
    public async Task Get_ShouldFail_WhenAbsent()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var absentBusinessObject = "absent-business-object";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/a1/business/business-objects/{absentBusinessObject}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Get_ShouldFail_WhenInvalid()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var invalidBusinessObject = "Invalid";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/a1/business/business-objects/{invalidBusinessObject}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}