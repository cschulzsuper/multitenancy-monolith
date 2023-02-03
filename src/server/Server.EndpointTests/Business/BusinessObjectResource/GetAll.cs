﻿using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary;
using ChristianSchulz.MultitenancyMonolith.Objects.Administration;
using ChristianSchulz.MultitenancyMonolith.Objects.Business;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ChristianSchulz.MultitenancyMonolith.Server.EndpointTests.Business.BusinessObjectResource;

public sealed class GetAll : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public GetAll(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithInMemoryData();
    }

    [Fact]
    [Trait("Category", "Endpoint.Security")]
    public async Task GetAll_ShouldBeUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/business/business-objects");

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [Trait("Category", "Endpoint.Security")]
    [InlineData(TestConfiguration.AdminIdentity)]
    [InlineData(TestConfiguration.DefaultIdentity)]
    [InlineData(TestConfiguration.GuestIdentity)]
    public async Task GetAll_ShouldBeForbidden_WhenNotAuthorized(string identity)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/business/business-objects");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.Group1, TestConfiguration.Group1Member)]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.Group2, TestConfiguration.Group2Member)]
    [InlineData(TestConfiguration.DefaultIdentity, TestConfiguration.Group1, TestConfiguration.Group1Member)]
    [InlineData(TestConfiguration.DefaultIdentity, TestConfiguration.Group2, TestConfiguration.Group2Member)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.Group1, TestConfiguration.Group1Member)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.Group2, TestConfiguration.Group2Member)]
    public async Task GetAll_ShouldSucceed_WithoutCustomProperties(string identity, string group, string member)
    {
        // Arrange
        var existingBusinessObject1 = new BusinessObject
        {
            Snowflake = 1,
            UniqueName = $"existing-business-object-1-{Guid.NewGuid()}"
        };

        var existingBusinessObject2 = new BusinessObject
        {
            Snowflake = 2,
            UniqueName = $"existing-business-object-2-{Guid.NewGuid()}"
        };

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<BusinessObject>>()
                .Insert(existingBusinessObject1, existingBusinessObject2);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/business/business-objects");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var client = _factory.CreateClient();

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

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.Group1, TestConfiguration.Group1Member)]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.Group2, TestConfiguration.Group2Member)]
    [InlineData(TestConfiguration.DefaultIdentity, TestConfiguration.Group1, TestConfiguration.Group1Member)]
    [InlineData(TestConfiguration.DefaultIdentity, TestConfiguration.Group2, TestConfiguration.Group2Member)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.Group1, TestConfiguration.Group1Member)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.Group2, TestConfiguration.Group2Member)]
    public async Task GetAll_ShouldSucceed_WithCustomProperties(string identity, string group, string member)
    {
        // Arrange
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
            Snowflake = 1,
            UniqueName = "business-object",
            CustomProperties = new List<ObjectTypeCustomProperty>
            {
                existingObjectTypeCustomProperty1, existingObjectTypeCustomProperty2
            }
        };

        var existingBusinessObject1 = new BusinessObject
        {
            Snowflake = 1,
            UniqueName = $"existing-business-object-1-{Guid.NewGuid()}",
            CustomProperties = new Dictionary<string, object>
            {
                ["customProperty1"] = "custom-property-value-1"
            }
        };

        var existingBusinessObject2 = new BusinessObject
        {
            Snowflake = 2,
            UniqueName = $"existing-business-object-2-{Guid.NewGuid()}",
            CustomProperties = new Dictionary<string, object>
            {
                ["customProperty2"] = "custom-property-value-2"
            }
        };

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);

            scope.ServiceProvider
                .GetRequiredService<IRepository<BusinessObject>>()
                .Insert(existingBusinessObject1, existingBusinessObject2);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/business/business-objects");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

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