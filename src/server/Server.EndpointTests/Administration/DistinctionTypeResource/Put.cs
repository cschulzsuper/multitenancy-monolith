using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Administration;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary;
using Xunit;

namespace ChristianSchulz.MultitenancyMonolith.Server.EndpointTests.Administration.DistinctionTypeResource;

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
        var validDistinctionType = "valid-distinction-type";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/administration/distinction-types/{validDistinctionType}");

        var putDistinctionType = new
        {
            UniqueName = "put-distinction-type",
            ObjectType = "business-object",
            DisplayName = "Put Distinction Type"
        };

        request.Content = JsonContent.Create(putDistinctionType);

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
        var validDistinctionType = "valid-distinction-type";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/administration/distinction-types/{validDistinctionType}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var putDistinctionType = new
        {
            UniqueName = "put-distinction-type",
            ObjectType = "business-object",
            DisplayName = "Put Distinction Type"
        };

        request.Content = JsonContent.Create(putDistinctionType);

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
        var validDistinctionType = "valid-distinction-type";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/administration/distinction-types/{validDistinctionType}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var putDistinctionType = new
        {
            UniqueName = "put-distinction-type",
            ObjectType = "business-object",
            DisplayName = "Put Distinction Type"
        };

        request.Content = JsonContent.Create(putDistinctionType);

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
    public async Task Put_ShouldSucceed_WhenObjectTypeBusinessObject(string identity, string group, string member)
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

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/administration/distinction-types/{existingDistinctionType.UniqueName}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var putDistinctionType = new
        {
            UniqueName = $"put-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Put Distinction Type"
        };

        request.Content = JsonContent.Create(putDistinctionType);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            var changedDistinctionType = scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingDistinctionType.Snowflake &&
                    x.UniqueName == putDistinctionType.UniqueName &&
                    x.ObjectType == putDistinctionType.ObjectType &&
                    x.DisplayName == putDistinctionType.DisplayName);

            Assert.NotNull(changedDistinctionType);
        }
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Put_ShouldFail_WhenInvalid(string identity, string group, string member)
    {
        // Arrange
        var invalidDistinctionType = "Invalid";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/administration/distinction-types/{invalidDistinctionType}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var putDistinctionType = new
        {
            UniqueName = "put-distinction-type",
            ObjectType = "business-object",
            DisplayName = "Put Distinction Type"
        };

        request.Content = JsonContent.Create(putDistinctionType);

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
        var absentDistinctionType = "absent-distinction-type";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/administration/distinction-types/{absentDistinctionType}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var putDistinctionType = new
        {
            UniqueName = "put-distinction-type",
            ObjectType = "business-object",
            DisplayName = "Put Distinction Type"
        };

        request.Content = JsonContent.Create(putDistinctionType);

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
        var existingDistinctionType = new DistinctionType
        {
            Snowflake = 1,
            UniqueName = $"existing-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type"
        };

        var additionalDistinctionType = new DistinctionType
        {
            Snowflake = 2,
            UniqueName = $"additional-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Additional Distinction Type"
        };

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType, additionalDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/administration/distinction-types/{existingDistinctionType.UniqueName}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var putDistinctionType = new
        {
            additionalDistinctionType.UniqueName,
            ObjectType = "business-object",
            DisplayName = "Put Distinction Type"
        };

        request.Content = JsonContent.Create(putDistinctionType);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

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
    public async Task Put_ShouldFail_WhenUniqueNameNull(string identity, string group, string member)
    {
        // Arrange
        var validDistinctionType = "valid-distinction-type";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/administration/distinction-types/{validDistinctionType}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var putDistinctionType = new
        {
            UniqueName = (string?) null,
            ObjectType = "business-object",
            DisplayName = "Put Distinction Type"
        };

        request.Content = JsonContent.Create(putDistinctionType);

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
        var validDistinctionType = "valid-distinction-type";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/administration/distinction-types/{validDistinctionType}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var putDistinctionType = new
        {
            UniqueName = string.Empty,
            ObjectType = "business-object",
            DisplayName = "Put Distinction Type"
        };

        request.Content = JsonContent.Create(putDistinctionType);

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
        var validDistinctionType = "valid-distinction-type";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/administration/distinction-types/{validDistinctionType}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var putDistinctionType = new
        {
            UniqueName = new string(Enumerable.Repeat('a', 141).ToArray()),
            ObjectType = "business-object",
            DisplayName = "Put Distinction Type"
        };

        request.Content = JsonContent.Create(putDistinctionType);

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
        var validDistinctionType = "valid-distinction-type";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/administration/distinction-types/{validDistinctionType}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var putDistinctionType = new
        {
            UniqueName = "Invalid",
            ObjectType = "business-object",
            DisplayName = "Put Distinction Type"
        };

        request.Content = JsonContent.Create(putDistinctionType);

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
    public async Task Put_ShouldFail_WhenDisplayNameNull(string identity, string group, string member)
    {
        // Arrange
        var validDistinctionType = "valid-distinction-type";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/administration/distinction-types/{validDistinctionType}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var putDistinctionType = new
        {
            UniqueName = "put-distinction-type",
            ObjectType = "business-object",
            DisplayName = (string?) null,
        };

        request.Content = JsonContent.Create(putDistinctionType);

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
    public async Task Put_ShouldFail_WhenDisplayNameEmpty(string identity, string group, string member)
    {
        // Arrange
        var validDistinctionType = "valid-distinction-type";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/administration/distinction-types/{validDistinctionType}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var putDistinctionType = new
        {
            UniqueName = "put-distinction-type",
            ObjectType = "business-object",
            DisplayName = string.Empty
        };

        request.Content = JsonContent.Create(putDistinctionType);

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
    public async Task Put_ShouldFail_WhenDisplayNameTooLong(string identity, string group, string member)
    {
        // Arrange
        var validDistinctionType = "valid-distinction-type";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/administration/distinction-types/{validDistinctionType}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var putDistinctionType = new
        {
            UniqueName = "put-distinction-type",
            ObjectType = "put-info@localhost",
            DisplayName = new string(Enumerable.Repeat('a', 141).ToArray()),
        };

        request.Content = JsonContent.Create(putDistinctionType);

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
    public async Task Put_ShouldFail_WhenObjectTypeNull(string identity, string group, string member)
    {
        // Arrange
        var validDistinctionType = "valid-distinction-type";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/administration/distinction-types/{validDistinctionType}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var putDistinctionType = new
        {
            UniqueName = "put-distinction-type",
            ObjectType = (string?) null,
            DisplayName = "New Distinction Type"
        };

        request.Content = JsonContent.Create(putDistinctionType);

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
    public async Task Put_ShouldFail_WhenObjectTypeEmpty(string identity, string group, string member)
    {
        // Arrange
        var validDistinctionType = "valid-distinction-type";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/administration/distinction-types/{validDistinctionType}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var putDistinctionType = new
        {
            UniqueName = "put-distinction-type",
            ObjectType = string.Empty,
            DisplayName = "New Distinction Type"
        };

        request.Content = JsonContent.Create(putDistinctionType);

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
    public async Task Put_ShouldFailWhenObjectTypeInvalid(string identity, string group, string member)
    {
        // Arrange
        var validDistinctionType = "valid-distinction-type";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/administration/distinction-types/{validDistinctionType}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var putDistinctionType = new
        {
            UniqueName = "put-distinction-type",
            ObjectType = "foo-bar",
            DisplayName = "New Distinction Type"
        };

        request.Content = JsonContent.Create(putDistinctionType);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}