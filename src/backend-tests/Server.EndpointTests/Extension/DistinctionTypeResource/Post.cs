﻿using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Extension;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;

namespace Extension.DistinctionTypeResource;

public sealed class Post 
{
    [Fact]
    public async Task Post_ShouldSucceed_WhenObjectTypeBusinessObject()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/extension/distinction-types");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postDistinctionType = new
        {
            UniqueName = $"post-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Post Distinction Type"
        };

        request.Content = JsonContent.Create(postDistinctionType);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(content);
        Assert.Collection(content.OrderBy(x => x.Key),
            x => Assert.Equal(("displayName", postDistinctionType.DisplayName), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("objectType", postDistinctionType.ObjectType), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("uniqueName", postDistinctionType.UniqueName), (x.Key, (string?)x.Value)));

        using (var scope = application.CreateMultitenancyScope())
        {
            var createdDistinctionType = scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.UniqueName == postDistinctionType.UniqueName &&
                    x.ObjectType == postDistinctionType.ObjectType &&
                    x.DisplayName == postDistinctionType.DisplayName);

            Assert.NotNull(createdDistinctionType);
        }
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameExists()
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

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/extension/distinction-types");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postDistinctionType = new
        {
            existingDistinctionType.UniqueName,
            ObjectType = "business-object",
            DisplayName = "Post Distinction Type"
        };

        request.Content = JsonContent.Create(postDistinctionType);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = application.CreateMultitenancyScope())
        {
            var unchangedDistinctionType = scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingDistinctionType.Snowflake &&
                    x.UniqueName == existingDistinctionType.UniqueName &&
                    x.ObjectType == existingDistinctionType.ObjectType &&
                    x.DisplayName == existingDistinctionType.DisplayName);

            Assert.NotNull(unchangedDistinctionType);
        }
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameNull()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/extension/distinction-types");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postDistinctionType = new
        {
            UniqueName = (string?)null,
            ObjectType = "business-object",
            DisplayName = "Post Distinction Type"
        };

        request.Content = JsonContent.Create(postDistinctionType);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdDistinctionType = scope.ServiceProvider
            .GetRequiredService<IRepository<DistinctionType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postDistinctionType.UniqueName);

        Assert.Null(createdDistinctionType);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameEmpty()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/extension/distinction-types");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postDistinctionType = new
        {
            UniqueName = string.Empty,
            ObjectType = "business-object",
            DisplayName = "Post Distinction Type"
        };

        request.Content = JsonContent.Create(postDistinctionType);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdDistinctionType = scope.ServiceProvider
            .GetRequiredService<IRepository<DistinctionType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postDistinctionType.UniqueName);

        Assert.Null(createdDistinctionType);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameTooLong()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/extension/distinction-types");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postDistinctionType = new
        {
            UniqueName = new string(Enumerable.Repeat('a', 141).ToArray()),
            ObjectType = "business-object",
            DisplayName = "Post Distinction Type"
        };

        request.Content = JsonContent.Create(postDistinctionType);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdDistinctionType = scope.ServiceProvider
            .GetRequiredService<IRepository<DistinctionType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postDistinctionType.UniqueName);

        Assert.Null(createdDistinctionType);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameInvalid()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/extension/distinction-types");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postDistinctionType = new
        {
            UniqueName = "Invalid",
            ObjectType = "business-object",
            DisplayName = "Post Distinction Type"
        };

        request.Content = JsonContent.Create(postDistinctionType);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdDistinctionType = scope.ServiceProvider
            .GetRequiredService<IRepository<DistinctionType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postDistinctionType.UniqueName);

        Assert.Null(createdDistinctionType);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenDisplayNameNull()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/extension/distinction-types");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postDistinctionType = new
        {
            UniqueName = "post-distinction-type",
            ObjectType = "business-object",
            DisplayName = (string?)null,
        };

        request.Content = JsonContent.Create(postDistinctionType);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdDistinctionType = scope.ServiceProvider
            .GetRequiredService<IRepository<DistinctionType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postDistinctionType.UniqueName);

        Assert.Null(createdDistinctionType);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenDisplayNameEmpty()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/extension/distinction-types");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postDistinctionType = new
        {
            UniqueName = "post-distinction-type",
            ObjectType = "business-object",
            DisplayName = string.Empty
        };

        request.Content = JsonContent.Create(postDistinctionType);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdDistinctionType = scope.ServiceProvider
            .GetRequiredService<IRepository<DistinctionType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postDistinctionType.UniqueName);

        Assert.Null(createdDistinctionType);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenDisplayNameTooLong()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/extension/distinction-types");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postDistinctionType = new
        {
            UniqueName = "post-distinction-type",
            ObjectType = "business-object",
            DisplayName = new string(Enumerable.Repeat('a', 141).ToArray())
        };

        request.Content = JsonContent.Create(postDistinctionType);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdDistinctionType = scope.ServiceProvider
            .GetRequiredService<IRepository<DistinctionType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postDistinctionType.UniqueName);

        Assert.Null(createdDistinctionType);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenObjectTypeNull()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/extension/distinction-types");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postDistinctionType = new
        {
            UniqueName = "post-distinction-type",
            ObjectType = (string?)null,
            DisplayName = "Post Distinction Type"
        };

        request.Content = JsonContent.Create(postDistinctionType);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdDistinctionType = scope.ServiceProvider
            .GetRequiredService<IRepository<DistinctionType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postDistinctionType.UniqueName);

        Assert.Null(createdDistinctionType);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenMailObjectTypeEmpty()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/extension/distinction-types");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postDistinctionType = new
        {
            UniqueName = "post-distinction-type",
            ObjectType = string.Empty,
            DisplayName = "Post Distinction Type"
        };

        request.Content = JsonContent.Create(postDistinctionType);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdDistinctionType = scope.ServiceProvider
            .GetRequiredService<IRepository<DistinctionType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postDistinctionType.UniqueName);

        Assert.Null(createdDistinctionType);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenObjectTypeInvalid()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/extension/distinction-types");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postDistinctionType = new
        {
            UniqueName = "post-distinction-type",
            ObjectType = "foo-bar",
            DisplayName = "Post Distinction Type"
        };

        request.Content = JsonContent.Create(postDistinctionType);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdDistinctionType = scope.ServiceProvider
            .GetRequiredService<IRepository<DistinctionType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postDistinctionType.UniqueName);

        Assert.Null(createdDistinctionType);
    }
}