﻿using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Business;
using ChristianSchulz.MultitenancyMonolith.Objects.Extension;
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

public sealed class Post
{
    [Fact]
    public async Task Post_ShouldSucceed_WithoutCustomProperties()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/business/business-objects");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postBusinessObject = new
        {
            UniqueName = $"post-business-object-{Guid.NewGuid()}",
            CustomProperties = new Dictionary<string, object>()
        };

        request.Content = JsonContent.Create(postBusinessObject);

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
            x => Assert.Equal(("uniqueName", postBusinessObject.UniqueName), (x.Key, (string?)x.Value)));

        using (var scope = application.CreateMultitenancyScope())
        {
            var createdBusinessObject = scope.ServiceProvider
                .GetRequiredService<IRepository<BusinessObject>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.UniqueName == postBusinessObject.UniqueName);

            Assert.NotNull(createdBusinessObject);
        }
    }

    [Fact]
    public async Task Post_ShouldSucceed_WhenCustomPropertyUnknown()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/business/business-objects");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postBusinessObject = new
        {
            UniqueName = $"post-business-object-{Guid.NewGuid()}",
            CustomProperties = new Dictionary<string, object>
            {
                ["customProperty"] = "custom-property-value"
            }
        };

        request.Content = JsonContent.Create(postBusinessObject);

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
            x => Assert.Equal(("uniqueName", postBusinessObject.UniqueName), (x.Key, (string?)x.Value)));

        using (var scope = application.CreateMultitenancyScope())
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

    [Fact]
    public async Task Post_ShouldSucceed_WhenCustomPropertyIsString()
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

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/business/business-objects");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postBusinessObject = new
        {
            UniqueName = $"post-business-object-{Guid.NewGuid()}",
            CustomProperties = new Dictionary<string, object>
            {
                [existingObjectTypeCustomProperty.PropertyName] = "custom-property-value"
            }
        };

        request.Content = JsonContent.Create(postBusinessObject);

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
                    y => { Assert.Equal((existingObjectTypeCustomProperty.PropertyName, "custom-property-value"), (y.Key, y.Value?.GetValue<string>())); });
            },
            x => Assert.Equal(("uniqueName", postBusinessObject.UniqueName), (x.Key, (string?)x.Value)));

        using (var scope = application.CreateMultitenancyScope())
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

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameExists()
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

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/business/business-objects");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postBusinessObject = new
        {
            existingBusinessObject.UniqueName,
            CustomProperties = new Dictionary<string, object>()
        };

        request.Content = JsonContent.Create(postBusinessObject);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = application.CreateMultitenancyScope())
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

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameNull()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/business/business-objects");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postBusinessObject = new
        {
            UniqueName = (string?)null,
            CustomProperties = new Dictionary<string, object>()
        };

        request.Content = JsonContent.Create(postBusinessObject);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdBusinessObject = scope.ServiceProvider
            .GetRequiredService<IRepository<BusinessObject>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postBusinessObject.UniqueName);

        Assert.Null(createdBusinessObject);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameEmpty()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/business/business-objects");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postBusinessObject = new
        {
            UniqueName = string.Empty,
            CustomProperties = new Dictionary<string, object>()
        };

        request.Content = JsonContent.Create(postBusinessObject);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdBusinessObject = scope.ServiceProvider
            .GetRequiredService<IRepository<BusinessObject>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postBusinessObject.UniqueName);

        Assert.Null(createdBusinessObject);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameTooLong()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/business/business-objects");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postBusinessObject = new
        {
            UniqueName = new string(Enumerable.Repeat('a', 141).ToArray()),
            CustomProperties = new Dictionary<string, object>()
        };

        request.Content = JsonContent.Create(postBusinessObject);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdBusinessObject = scope.ServiceProvider
            .GetRequiredService<IRepository<BusinessObject>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postBusinessObject.UniqueName);

        Assert.Null(createdBusinessObject);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameInvalid()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/business/business-objects");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postBusinessObject = new
        {
            UniqueName = "Invalid",
            CustomProperties = new Dictionary<string, object>()
        };

        request.Content = JsonContent.Create(postBusinessObject);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdBusinessObject = scope.ServiceProvider
            .GetRequiredService<IRepository<BusinessObject>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postBusinessObject.UniqueName);

        Assert.Null(createdBusinessObject);
    }
}