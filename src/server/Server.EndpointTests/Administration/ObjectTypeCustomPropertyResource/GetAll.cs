﻿using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Administration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace ChristianSchulz.MultitenancyMonolith.Server.EndpointTests.Administration.ObjectTypeCustomPropertyResource;

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
        var validObjectType = "valid-object-type";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/administration/object-types/{validObjectType}/custom-properties");

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
        var validObjectType = "valid-object-type";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/administration/object-types/{validObjectType}/custom-properties");
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
    public async Task GetAll_ShouldSucceed(string identity, string group, string member)
    {
        // Arrange
        var existingObjectTypeCustomProperty1 = new ObjectTypeCustomProperty
        {
            UniqueName = $"existing-object-type-custom-property-1-{Guid.NewGuid()}",
            DisplayName = "Foo Bar",
            PropertyName = "fooBar",
            PropertyType = "string"
        };

        var existingObjectTypeCustomProperty2 = new ObjectTypeCustomProperty
        {
            UniqueName = $"existing-object-type-custom-property-2-{Guid.NewGuid()}",
            DisplayName = "Foo Bar",
            PropertyName = "fooBar",
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

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/administration/object-types/{existingObjectType.UniqueName}/custom-properties");
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

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.Group1, TestConfiguration.Group1Member)]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.Group2, TestConfiguration.Group2Member)]
    [InlineData(TestConfiguration.DefaultIdentity, TestConfiguration.Group1, TestConfiguration.Group1Member)]
    [InlineData(TestConfiguration.DefaultIdentity, TestConfiguration.Group2, TestConfiguration.Group2Member)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.Group1, TestConfiguration.Group1Member)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.Group2, TestConfiguration.Group2Member)]
    public async Task Get_ShouldFail_WhenInvalidObjectType(string identity, string group, string member)
    {
        // Arrange
        var invalidObjectType = "Invalid-object-type";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/administration/object-types/{invalidObjectType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}