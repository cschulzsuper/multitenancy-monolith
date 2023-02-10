using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary;
using ChristianSchulz.MultitenancyMonolith.Objects.Administration;
using ChristianSchulz.MultitenancyMonolith.Objects.Business;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ChristianSchulz.MultitenancyMonolith.Server.EndpointTests.Business.BusinessObjectResource;

public sealed class Post : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Post(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithInMemoryData();
    }

    [Fact]
    [Trait("Category", "Endpoint.Security")]
    public async Task Post_ShouldBeUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/business/business-objects");

        var postBusinessObject = new
        {
            UniqueName = "post-business-object"
        };

        request.Content = JsonContent.Create(postBusinessObject);

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
    public async Task Post_ShouldBeForbidden_WhenNotAuthorized(string identity)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/business/business-objects");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var postBusinessObject = new
        {
            UniqueName = "post-business-object",
            CustomProperties = new Dictionary<string, object>()
        };

        request.Content = JsonContent.Create(postBusinessObject);

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
    public async Task Post_ShouldBeForbidden_WhenNotChief(string identity, string group, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/business/business-objects");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postBusinessObject = new
        {
            UniqueName = "post-business-object",
            CustomProperties = new Dictionary<string, object>()
        };

        request.Content = JsonContent.Create(postBusinessObject);

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
    public async Task Post_ShouldSucceed_WithoutCustomProperties(string identity, string group, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/business/business-objects");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postBusinessObject = new
        {
            UniqueName = $"post-business-object-{Guid.NewGuid()}",
            CustomProperties = new Dictionary<string, object>()
        };

        request.Content = JsonContent.Create(postBusinessObject);

        var client = _factory.CreateClient();

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
            x => Assert.Equal(("uniqueName", postBusinessObject.UniqueName), (x.Key, (string?) x.Value)));

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            var createdBusinessObject = scope.ServiceProvider
                .GetRequiredService<IRepository<BusinessObject>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.UniqueName == postBusinessObject.UniqueName);

            Assert.NotNull(createdBusinessObject);
        }
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldSucceed_WhenCustomPropertyUnknown(string identity, string group, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/business/business-objects");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postBusinessObject = new
        {
            UniqueName = $"post-business-object-{Guid.NewGuid()}",
            CustomProperties = new Dictionary<string, object>
            {
                ["customProperty"] = "custom-property-value"
            }
        };

        request.Content = JsonContent.Create(postBusinessObject);

        var client = _factory.CreateClient();

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
            x => Assert.Equal(("uniqueName", postBusinessObject.UniqueName), (x.Key, (string?) x.Value)));

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            var createdBusinessObject = scope.ServiceProvider
                .GetRequiredService<IRepository<BusinessObject>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.UniqueName == postBusinessObject.UniqueName &&
                    !x.CustomProperties.Any());

            Assert.NotNull(createdBusinessObject);
        }
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldSucceed_WhenCustomPropertyIsString(string identity, string group, string member)
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

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/business/business-objects");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postBusinessObject = new
        {
            UniqueName = $"post-business-object-{Guid.NewGuid()}",
            CustomProperties = new Dictionary<string, object>
            {
                [existingObjectTypeCustomProperty.PropertyName] = "custom-property-value"
            }
        };

        request.Content = JsonContent.Create(postBusinessObject);

        var client = _factory.CreateClient();

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
                    y => { Assert.Equal((existingObjectTypeCustomProperty.PropertyName, "custom-property-value"), (y.Key, y.Value?.GetValue<string>())); });
            },
            x => Assert.Equal(("uniqueName", postBusinessObject.UniqueName), (x.Key, (string?) x.Value)));

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            var createdBusinessObject = scope.ServiceProvider
                .GetRequiredService<IRepository<BusinessObject>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.UniqueName == postBusinessObject.UniqueName &&
                    x.CustomProperties.Count == 1 &&
                    x.CustomProperties.Any(y =>
                        y.Key == existingObjectTypeCustomProperty.PropertyName &&
                        y.Value.Equals("custom-property-value")));

            Assert.NotNull(createdBusinessObject);
        }
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldFail_WhenUniqueNameExists(string identity, string group, string member)
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

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/business/business-objects");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postBusinessObject = new
        {
            UniqueName = existingBusinessObject.UniqueName,
            CustomProperties = new Dictionary<string, object>()
        };

        request.Content = JsonContent.Create(postBusinessObject);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

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
    public async Task Post_ShouldFail_WhenUniqueNameNull(string identity, string group, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/business/business-objects");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postBusinessObject = new
        {
            UniqueName = (string?) null,
            CustomProperties = new Dictionary<string, object>()
        };

        request.Content = JsonContent.Create(postBusinessObject);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateMultitenancyScope(group);

        var createdBusinessObject = scope.ServiceProvider
            .GetRequiredService<IRepository<BusinessObject>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postBusinessObject.UniqueName);

        Assert.Null(createdBusinessObject);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldFail_WhenUniqueNameEmpty(string identity, string group, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/business/business-objects");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postBusinessObject = new
        {
            UniqueName = string.Empty,
            CustomProperties = new Dictionary<string, object>()
        };

        request.Content = JsonContent.Create(postBusinessObject);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateMultitenancyScope(group);

        var createdBusinessObject = scope.ServiceProvider
            .GetRequiredService<IRepository<BusinessObject>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postBusinessObject.UniqueName);

        Assert.Null(createdBusinessObject);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldFail_WhenUniqueNameTooLong(string identity, string group, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/business/business-objects");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postBusinessObject = new
        {
            UniqueName = new string(Enumerable.Repeat('a', 141).ToArray()),
            CustomProperties = new Dictionary<string, object>()
        };

        request.Content = JsonContent.Create(postBusinessObject);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateMultitenancyScope(group);

        var createdBusinessObject = scope.ServiceProvider
            .GetRequiredService<IRepository<BusinessObject>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postBusinessObject.UniqueName);

        Assert.Null(createdBusinessObject);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldFail_WhenUniqueNameInvalid(string identity, string group, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/business/business-objects");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postBusinessObject = new
        {
            UniqueName = "Invalid",
            CustomProperties = new Dictionary<string, object>()
        };

        request.Content = JsonContent.Create(postBusinessObject);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateMultitenancyScope(group);

        var createdBusinessObject = scope.ServiceProvider
            .GetRequiredService<IRepository<BusinessObject>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postBusinessObject.UniqueName);

        Assert.Null(createdBusinessObject);
    }
}