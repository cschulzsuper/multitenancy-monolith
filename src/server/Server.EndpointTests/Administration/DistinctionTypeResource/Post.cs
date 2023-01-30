using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Administration;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace ChristianSchulz.MultitenancyMonolith.Server.EndpointTests.Administration.DistinctionTypeResource;

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
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/administration/distinction-types");

        var postDistinctionType = new
        {
            UniqueName = "post-distinction-type",
            ObjectType = "business-object",
            DisplayName = "Post Distinction Type"
        };

        request.Content = JsonContent.Create(postDistinctionType);

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
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/administration/distinction-types");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var postDistinctionType = new
        {
            UniqueName = "post-distinction-type",
            ObjectType = "business-object",
            DisplayName = "Post Distinction Type"
        };

        request.Content = JsonContent.Create(postDistinctionType);

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
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/administration/distinction-types");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postDistinctionType = new
        {
            UniqueName = "post-distinction-type",
            ObjectType = "business-object",
            DisplayName = "Post Distinction Type"
        };

        request.Content = JsonContent.Create(postDistinctionType);

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
    public async Task Post_ShouldSucceed_WhenObjectTypeBusinessObject(string identity, string group, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/administration/distinction-types");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postDistinctionType = new
        {
            UniqueName = $"post-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Post Distinction Type"
        };

        request.Content = JsonContent.Create(postDistinctionType);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(content);
        Assert.Collection(content.OrderBy(x => x.Key),
            x => Assert.Equal((x.Key, (string?) x.Value), ("displayName", postDistinctionType.DisplayName)),
            x => Assert.Equal((x.Key, (string?) x.Value), ("objectType", postDistinctionType.ObjectType)),
            x => Assert.Equal((x.Key, (string?) x.Value), ("uniqueName", postDistinctionType.UniqueName)));

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
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

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldFail_WhenUniqueNameExists(string identity, string group, string member)
    {
        // Arrange
        var existingDistinctionType = new DistinctionType
        {
            Snowflake = 1,
            UniqueName = $"existing-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type"
        };

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/administration/distinction-types");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postDistinctionType = new
        {
            existingDistinctionType.UniqueName,
            ObjectType = "business-object",
            DisplayName = "Post Distinction Type"
        };

        request.Content = JsonContent.Create(postDistinctionType);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
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

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldFail_WhenUniqueNameNull(string identity, string group, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/administration/distinction-types");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postDistinctionType = new
        {
            UniqueName = (string?) null,
            ObjectType = "business-object",
            DisplayName = "Post Distinction Type"
        };

        request.Content = JsonContent.Create(postDistinctionType);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateMultitenancyScope(group);

        var createdDistinctionType = scope.ServiceProvider
            .GetRequiredService<IRepository<DistinctionType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postDistinctionType.UniqueName);

        Assert.Null(createdDistinctionType);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldFail_WhenUniqueNameEmpty(string identity, string group, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/administration/distinction-types");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postDistinctionType = new
        {
            UniqueName = string.Empty,
            ObjectType = "business-object",
            DisplayName = "Post Distinction Type"
        };

        request.Content = JsonContent.Create(postDistinctionType);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateMultitenancyScope(group);

        var createdDistinctionType = scope.ServiceProvider
            .GetRequiredService<IRepository<DistinctionType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postDistinctionType.UniqueName);

        Assert.Null(createdDistinctionType);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldFail_WhenUniqueNameTooLong(string identity, string group, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/administration/distinction-types");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postDistinctionType = new
        {
            UniqueName = new string(Enumerable.Repeat('a', 141).ToArray()),
            ObjectType = "business-object",
            DisplayName = "Post Distinction Type"
        };

        request.Content = JsonContent.Create(postDistinctionType);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateMultitenancyScope(group);

        var createdDistinctionType = scope.ServiceProvider
            .GetRequiredService<IRepository<DistinctionType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postDistinctionType.UniqueName);

        Assert.Null(createdDistinctionType);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldFail_WhenUniqueNameInvalid(string identity, string group, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/administration/distinction-types");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postDistinctionType = new
        {
            UniqueName = "Invalid",
            ObjectType = "business-object",
            DisplayName = "Post Distinction Type"
        };

        request.Content = JsonContent.Create(postDistinctionType);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateMultitenancyScope(group);

        var createdDistinctionType = scope.ServiceProvider
            .GetRequiredService<IRepository<DistinctionType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postDistinctionType.UniqueName);

        Assert.Null(createdDistinctionType);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldFail_WhenDisplayNameNull(string identity, string group, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/administration/distinction-types");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postDistinctionType = new
        {
            UniqueName = "post-distinction-type",
            ObjectType = "business-object",
            DisplayName = (string?) null,
        };

        request.Content = JsonContent.Create(postDistinctionType);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateMultitenancyScope(group);

        var createdDistinctionType = scope.ServiceProvider
            .GetRequiredService<IRepository<DistinctionType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postDistinctionType.UniqueName);

        Assert.Null(createdDistinctionType);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldFail_WhenDisplayNameEmpty(string identity, string group, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/administration/distinction-types");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postDistinctionType = new
        {
            UniqueName = "post-distinction-type",
            ObjectType = "business-object",
            DisplayName = string.Empty
        };

        request.Content = JsonContent.Create(postDistinctionType);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateMultitenancyScope(group);

        var createdDistinctionType = scope.ServiceProvider
            .GetRequiredService<IRepository<DistinctionType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postDistinctionType.UniqueName);

        Assert.Null(createdDistinctionType);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldFail_WhenDisplayNameTooLong(string identity, string group, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/administration/distinction-types");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postDistinctionType = new
        {
            UniqueName = "post-distinction-type",
            ObjectType = "business-object",
            DisplayName = new string(Enumerable.Repeat('a', 141).ToArray())
        };

        request.Content = JsonContent.Create(postDistinctionType);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateMultitenancyScope(group);

        var createdDistinctionType = scope.ServiceProvider
            .GetRequiredService<IRepository<DistinctionType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postDistinctionType.UniqueName);

        Assert.Null(createdDistinctionType);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldFail_WhenObjectTypeNull(string identity, string group, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/administration/distinction-types");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postDistinctionType = new
        {
            UniqueName = "post-distinction-type",
            ObjectType = (string?) null,
            DisplayName = "Post Distinction Type"
        };

        request.Content = JsonContent.Create(postDistinctionType);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateMultitenancyScope(group);

        var createdDistinctionType = scope.ServiceProvider
            .GetRequiredService<IRepository<DistinctionType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postDistinctionType.UniqueName);

        Assert.Null(createdDistinctionType);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldFail_WhenMailObjectTypeEmpty(string identity, string group, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/administration/distinction-types");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postDistinctionType = new
        {
            UniqueName = "post-distinction-type",
            ObjectType = string.Empty,
            DisplayName = "Post Distinction Type"
        };

        request.Content = JsonContent.Create(postDistinctionType);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateMultitenancyScope(group);

        var createdDistinctionType = scope.ServiceProvider
            .GetRequiredService<IRepository<DistinctionType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postDistinctionType.UniqueName);

        Assert.Null(createdDistinctionType);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldFail_WhenObjectTypeInvalid(string identity, string group, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/administration/distinction-types");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postDistinctionType = new
        {
            UniqueName = "post-distinction-type",
            ObjectType = "foo-bar",
            DisplayName = "Post Distinction Type"
        };

        request.Content = JsonContent.Create(postDistinctionType);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateMultitenancyScope(group);

        var createdDistinctionType = scope.ServiceProvider
            .GetRequiredService<IRepository<DistinctionType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postDistinctionType.UniqueName);

        Assert.Null(createdDistinctionType);
    }
}