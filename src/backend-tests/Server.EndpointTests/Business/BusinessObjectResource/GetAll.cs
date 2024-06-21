using ChristianSchulz.MultitenancyMonolith.Backend.Server;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Business;
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

namespace Business.BusinessObjectResource;

public sealed class GetAll 
{
    [Fact]
    public async Task GetAll_ShouldSucceed_WithoutCustomProperties()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingBusinessObject1 = new BusinessObject
        {
            UniqueName = $"existing-business-object-1-{Guid.NewGuid()}"
        };

        var existingBusinessObject2 = new BusinessObject
        {
            UniqueName = $"existing-business-object-2-{Guid.NewGuid()}"
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<BusinessObject>>()
                .Insert(existingBusinessObject1, existingBusinessObject2);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/a1/business/business-objects");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonDocument>();
        Assert.NotNull(content);
        Assert.Collection(content.RootElement.EnumerateArray().OrderBy(x => x.GetString("uniqueName")),
            x => { Assert.Equal(existingBusinessObject1.UniqueName, x.GetString("uniqueName")); },
            x => { Assert.Equal(existingBusinessObject2.UniqueName, x.GetString("uniqueName")); });
    }

    [Fact]
    public async Task GetAll_ShouldSucceed_WithCustomProperties()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingObjectTypeCustomProperty1 = new ObjectTypeCustomProperty
        {
            UniqueName = $"custom-property-1-{Guid.NewGuid()}",
            DisplayName = "Custom Property 1",
            PropertyName = "customProperty1",
            PropertyType = "string"
        };

        var existingObjectTypeCustomProperty2 = new ObjectTypeCustomProperty
        {
            UniqueName = $"custom-property-2-{Guid.NewGuid()}",
            DisplayName = "Custom Property 2",
            PropertyName = "customProperty2",
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

        var existingBusinessObject1 = new BusinessObject
        {
            UniqueName = $"existing-business-object-1-{Guid.NewGuid()}",
            CustomProperties = new Dictionary<string, object>
            {
                ["customProperty1"] = "custom-property-value-1"
            }
        };

        var existingBusinessObject2 = new BusinessObject
        {
            UniqueName = $"existing-business-object-2-{Guid.NewGuid()}",
            CustomProperties = new Dictionary<string, object>
            {
                ["customProperty2"] = "custom-property-value-2"
            }
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);

            scope.ServiceProvider
                .GetRequiredService<IRepository<BusinessObject>>()
                .Insert(existingBusinessObject1, existingBusinessObject2);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/a1/business/business-objects");
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
                Assert.Equal(existingBusinessObject1.UniqueName, x.GetString("uniqueName"));
                Assert.Equal(existingBusinessObject1.CustomProperties["customProperty1"],
                    x.GetProperty("customProperties").GetString("customProperty1"));
            },
            x =>
            {
                Assert.Equal(existingBusinessObject2.UniqueName, x.GetString("uniqueName"));
                Assert.Equal(existingBusinessObject2.CustomProperties["customProperty2"],
                    x.GetProperty("customProperties").GetString("customProperty2"));
            });
    }
}