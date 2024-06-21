using ChristianSchulz.MultitenancyMonolith.Backend.Server;
using ChristianSchulz.MultitenancyMonolith.Data;
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

namespace Extension.DistinctionTypeCustomPropertyResource;

public sealed class Post 
{
    [Fact]
    public async Task Post_ShouldSucceed_WhenValid()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingDistinctionType = new DistinctionType
        {
            UniqueName = $"existing-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type"
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/extension/distinction-types/{existingDistinctionType.UniqueName}/custom-properties");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postDistinctionTypeCustomProperty = new
        {
            UniqueName = $"post-distinction-type-custom-property-{Guid.NewGuid()}"
        };

        request.Content = JsonContent.Create(postDistinctionTypeCustomProperty);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(content);
        Assert.Collection(content.OrderBy(x => x.Key),
            x => Assert.Equal(("distinctionType", existingDistinctionType.UniqueName), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("uniqueName", postDistinctionTypeCustomProperty.UniqueName), (x.Key, (string?)x.Value)));

        using (var scope = application.CreateMultitenancyScope())
        {
            var createdDistinctionTypeCustomProperty1 = scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .GetQueryable()
                .Select(x => new { UniqueName = x.UniqueName, CustomProperties = x.CustomProperties })
                .Single(x => x.UniqueName == existingDistinctionType.UniqueName);
            var createdDistinctionTypeCustomProperty = createdDistinctionTypeCustomProperty1
                .CustomProperties
                .SingleOrDefault(x => x.UniqueName == postDistinctionTypeCustomProperty.UniqueName);

            Assert.NotNull(createdDistinctionTypeCustomProperty);
        }
    }

    [Fact]
    public async Task Post_ShouldFail_WhenInvalidDistinctionType()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var invalidDistinctionType = "Invalid";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/extension/distinction-types/{invalidDistinctionType}/custom-properties");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postDistinctionTypeCustomProperty = new
        {
            UniqueName = "post-distinction-type-custom-property"
        };

        request.Content = JsonContent.Create(postDistinctionTypeCustomProperty);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameExists()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingDistinctionTypeCustomProperty = new DistinctionTypeCustomProperty
        {
            UniqueName = $"existing-distinction-type-custom-property-{Guid.NewGuid()}"
        };

        var existingDistinctionType = new DistinctionType
        {
            UniqueName = $"existing-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type",
            CustomProperties = new List<DistinctionTypeCustomProperty>
        {
            existingDistinctionTypeCustomProperty
        }
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/extension/distinction-types/{existingDistinctionType.UniqueName}/custom-properties");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postDistinctionTypeCustomProperty = new
        {
            existingDistinctionTypeCustomProperty.UniqueName
        };

        request.Content = JsonContent.Create(postDistinctionTypeCustomProperty);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        var c = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameNull()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingDistinctionType = new DistinctionType
        {
            UniqueName = $"existing-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type"
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/extension/distinction-types/{existingDistinctionType.UniqueName}/custom-properties");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postDistinctionTypeCustomProperty = new
        {
            UniqueName = (string?)null
        };

        request.Content = JsonContent.Create(postDistinctionTypeCustomProperty);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = application.CreateMultitenancyScope())
        {
            var createdDistinctionTypeCustomProperty = scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .GetQueryable()
                .Single(x =>
                    x.Snowflake == existingDistinctionType.Snowflake &&
                    x.UniqueName == existingDistinctionType.UniqueName)
                .CustomProperties
                .SingleOrDefault(x => x.UniqueName == postDistinctionTypeCustomProperty.UniqueName);

            Assert.Null(createdDistinctionTypeCustomProperty);
        }
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameEmpty()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingDistinctionType = new DistinctionType
        {
            UniqueName = $"existing-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type"
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/extension/distinction-types/{existingDistinctionType.UniqueName}/custom-properties");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postDistinctionTypeCustomProperty = new
        {
            UniqueName = string.Empty
        };

        request.Content = JsonContent.Create(postDistinctionTypeCustomProperty);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = application.CreateMultitenancyScope())
        {
            var createdDistinctionTypeCustomProperty = scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .GetQueryable()
                .Single(x =>
                    x.Snowflake == existingDistinctionType.Snowflake &&
                    x.UniqueName == existingDistinctionType.UniqueName)
                .CustomProperties
                .SingleOrDefault(x => x.UniqueName == postDistinctionTypeCustomProperty.UniqueName);

            Assert.Null(createdDistinctionTypeCustomProperty);
        }
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameTooLong()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingDistinctionType = new DistinctionType
        {
            UniqueName = $"existing-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type"
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/extension/distinction-types/{existingDistinctionType.UniqueName}/custom-properties");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postDistinctionTypeCustomProperty = new
        {
            UniqueName = new string(Enumerable.Repeat('a', 141).ToArray())
        };

        request.Content = JsonContent.Create(postDistinctionTypeCustomProperty);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = application.CreateMultitenancyScope())
        {
            var createdDistinctionTypeCustomProperty = scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .GetQueryable()
                .Single(x =>
                    x.Snowflake == existingDistinctionType.Snowflake &&
                    x.UniqueName == existingDistinctionType.UniqueName)
                .CustomProperties
                .SingleOrDefault(x => x.UniqueName == postDistinctionTypeCustomProperty.UniqueName);

            Assert.Null(createdDistinctionTypeCustomProperty);
        }
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameInvalid()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingDistinctionType = new DistinctionType
        {
            UniqueName = $"existing-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type"
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/extension/distinction-types/{existingDistinctionType.UniqueName}/custom-properties");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postDistinctionTypeCustomProperty = new
        {
            UniqueName = "Invalid"
        };

        request.Content = JsonContent.Create(postDistinctionTypeCustomProperty);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = application.CreateMultitenancyScope())
        {
            var createdDistinctionTypeCustomProperty = scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .GetQueryable()
                .Single(x =>
                    x.Snowflake == existingDistinctionType.Snowflake &&
                    x.UniqueName == existingDistinctionType.UniqueName)
                .CustomProperties
                .SingleOrDefault(x => x.UniqueName == postDistinctionTypeCustomProperty.UniqueName);

            Assert.Null(createdDistinctionTypeCustomProperty);
        }
    }
}