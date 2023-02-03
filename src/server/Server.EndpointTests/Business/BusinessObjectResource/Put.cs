﻿using System.Net;
using System.Net.Http.Json;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary;
using ChristianSchulz.MultitenancyMonolith.Objects.Administration;
using ChristianSchulz.MultitenancyMonolith.Objects.Business;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ChristianSchulz.MultitenancyMonolith.Server.EndpointTests.Business.BusinessObjectResource;

public sealed class Put : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Put(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithInMemoryData();
    }

    [Fact]
    [Trait("Category", "Endpoint.Security")]
    public async Task Put_ShouldBeUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        var validBusinessObject = "valid-business-object";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/business/business-objects/{validBusinessObject}");

        var putBusinessObject = new
        {
            UniqueName = "put-business-object"
        };

        request.Content = JsonContent.Create(putBusinessObject);

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
    public async Task Put_ShouldBeForbidden_WhenNotAuthorized(string identity)
    {
        // Arrange
        var validBusinessObject = "valid-business-object";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/business/business-objects/{validBusinessObject}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var putBusinessObject = new
        {
            UniqueName = "put-business-object"
        };

        request.Content = JsonContent.Create(putBusinessObject);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [Trait("Category", "Endpoint.Security")]
    [InlineData(TestConfiguration.DefaultIdentity, TestConfiguration.Group1, TestConfiguration.Group1Member)]
    [InlineData(TestConfiguration.DefaultIdentity, TestConfiguration.Group2, TestConfiguration.Group2Member)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.Group1, TestConfiguration.Group1Member)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.Group2, TestConfiguration.Group2Member)]
    public async Task Put_ShouldBeForbidden_WhenNotChief(string identity, string group, string member)
    {
        // Arrange
        var validBusinessObject = "valid-business-object";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/business/business-objects/{validBusinessObject}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var putBusinessObject = new
        {
            UniqueName = "put-business-object"
        };

        request.Content = JsonContent.Create(putBusinessObject);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Put_ShouldSucceed_WithoutCustomProperties(string identity, string group, string member)
    {
        // Arrange
        var existingBusinessObject = new BusinessObject
        {
            Snowflake = 1,
            UniqueName = $"existing-business-object-{Guid.NewGuid()}"
        };

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<BusinessObject>>()
                .Insert(existingBusinessObject);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/business/business-objects/{existingBusinessObject.UniqueName}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var putBusinessObject = new
        {
            UniqueName = $"put-business-object-{Guid.NewGuid()}",
            CustomProperties = new Dictionary<string, object>()
        };

        request.Content = JsonContent.Create(putBusinessObject);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            var changedBusinessObject = scope.ServiceProvider
                .GetRequiredService<IRepository<BusinessObject>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingBusinessObject.Snowflake &&
                    x.UniqueName == putBusinessObject.UniqueName);

            Assert.NotNull(changedBusinessObject);
        }
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Put_ShouldSucceed_WhenCustomPropertyUnknown(string identity, string group, string member)
    {
        // Arrange
        var existingBusinessObject = new BusinessObject
        {
            Snowflake = 1,
            UniqueName = $"existing-business-object-{Guid.NewGuid()}"
        };

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<BusinessObject>>()
                .Insert(existingBusinessObject);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/business/business-objects/{existingBusinessObject.UniqueName}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var putBusinessObject = new
        {
            UniqueName = $"put-business-object-{Guid.NewGuid()}",
            CustomProperties = new Dictionary<string, object>
            {
                ["customProperty"] = "custom-property-value"
            }
        };

        request.Content = JsonContent.Create(putBusinessObject);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            var changedBusinessObject = scope.ServiceProvider
                .GetRequiredService<IRepository<BusinessObject>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingBusinessObject.Snowflake &&
                    x.UniqueName == putBusinessObject.UniqueName &&
                    !x.CustomProperties.Any());

            Assert.NotNull(changedBusinessObject);
        }
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Put_ShouldSucceed_WhenCustomPropertyIsString(string identity, string group, string member)
    {
        // Arrange
        var existingObjectTypeCustomProperty = new ObjectTypeCustomProperty
        {
            UniqueName = $"custom-property-{Guid.NewGuid()}",
            DisplayName = "Custom Property",
            PropertyName = "customProperty",
            PropertyType = "string"
        };

        var existingObjectType = new ObjectType
        {
            Snowflake = 1,
            UniqueName = "business-object",
            CustomProperties = new List<ObjectTypeCustomProperty>
            {
                existingObjectTypeCustomProperty
            }
        };

        var existingBusinessObject = new BusinessObject
        {
            Snowflake = 1,
            UniqueName = $"existing-business-object-{Guid.NewGuid()}"
        };

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);

            scope.ServiceProvider
                .GetRequiredService<IRepository<BusinessObject>>()
                .Insert(existingBusinessObject);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/business/business-objects/{existingBusinessObject.UniqueName}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var putBusinessObject = new
        {
            UniqueName = $"put-business-object-{Guid.NewGuid()}",
            CustomProperties = new Dictionary<string, object>
            {
                [existingObjectTypeCustomProperty.PropertyName] = "custom-property-value"
            }
        };

        request.Content = JsonContent.Create(putBusinessObject);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        var x = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            var changedBusinessObject = scope.ServiceProvider
                .GetRequiredService<IRepository<BusinessObject>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingBusinessObject.Snowflake &&
                    x.UniqueName == putBusinessObject.UniqueName &&
                    x.CustomProperties.Count == 1 &&
                    x.CustomProperties.Any(y =>
                        y.Key == existingObjectTypeCustomProperty.PropertyName &&
                        y.Value.Equals("custom-property-value")));

            Assert.NotNull(changedBusinessObject);
        }
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Put_ShouldFail_WhenInvalid(string identity, string group, string member)
    {
        // Arrange
        var invalidBusinessObject = "Invalid";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/business/business-objects/{invalidBusinessObject}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var putBusinessObject = new
        {
            UniqueName = "put-business-object"
        };

        request.Content = JsonContent.Create(putBusinessObject);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Put_ShouldFail_WhenAbsent(string identity, string group, string member)
    {
        // Arrange
        var absentBusinessObject = "absent-business-object";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/business/business-objects/{absentBusinessObject}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var putBusinessObject = new
        {
            UniqueName = "put-business-object"
        };

        request.Content = JsonContent.Create(putBusinessObject);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Put_ShouldFail_WhenUniqueNameExists(string identity, string group, string member)
    {
        // Arrange
        var existingBusinessObject = new BusinessObject
        {
            Snowflake = 1,
            UniqueName = $"existing-business-object-{Guid.NewGuid()}"
        };

        var additionalBusinessObject = new BusinessObject
        {
            Snowflake = 2,
            UniqueName = $"additional-business-object-{Guid.NewGuid()}"
        };

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<BusinessObject>>()
                .Insert(existingBusinessObject, additionalBusinessObject);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/business/business-objects/{existingBusinessObject.UniqueName}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var putBusinessObject = new
        {
            UniqueName = additionalBusinessObject.UniqueName
        };

        request.Content = JsonContent.Create(putBusinessObject);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            var unchangedBusinessObject = scope.ServiceProvider
                .GetRequiredService<IRepository<BusinessObject>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingBusinessObject.Snowflake &&
                    x.UniqueName == existingBusinessObject.UniqueName);

            Assert.NotNull(unchangedBusinessObject);
        }
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Put_ShouldFail_WhenUniqueNameNull(string identity, string group, string member)
    {
        // Arrange
        var validBusinessObject = "valid-business-object";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/business/business-objects/{validBusinessObject}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var putBusinessObject = new
        {
            UniqueName = (string?) null
        };

        request.Content = JsonContent.Create(putBusinessObject);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Put_ShouldFail_WhenUniqueNameEmpty(string identity, string group, string member)
    {
        // Arrange
        var validBusinessObject = "valid-business-object";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/business/business-objects/{validBusinessObject}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var putBusinessObject = new
        {
            UniqueName = string.Empty
        };

        request.Content = JsonContent.Create(putBusinessObject);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Put_ShouldFail_WhenUniqueNameTooLong(string identity, string group, string member)
    {
        // Arrange
        var validBusinessObject = "valid-business-object";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/business/business-objects/{validBusinessObject}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var putBusinessObject = new
        {
            UniqueName = new string(Enumerable.Repeat('a', 141).ToArray())
        };

        request.Content = JsonContent.Create(putBusinessObject);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Put_ShouldFail_WhenUniqueNameInvalid(string identity, string group, string member)
    {
        // Arrange
        var validBusinessObject = "valid-business-object";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/business/business-objects/{validBusinessObject}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var putBusinessObject = new
        {
            UniqueName = "Invalid"
        };

        request.Content = JsonContent.Create(putBusinessObject);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}