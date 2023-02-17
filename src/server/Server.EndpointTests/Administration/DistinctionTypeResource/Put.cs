using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Administration;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using System.Threading.Tasks;
using System.Net.Http;
using System.Linq;
using System;
using ChristianSchulz.MultitenancyMonolith.Server;

namespace Administration.DistinctionTypeResource;

public sealed class Put : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Put(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Put_ShouldSucceed_WhenObjectTypeBusinessObject()
    {
        // Arrange
        var existingDistinctionType = new DistinctionType
        {
            Snowflake = 1,
            UniqueName = $"existing-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/administration/distinction-types/{existingDistinctionType.UniqueName}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

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

        using (var scope = _factory.CreateMultitenancyScope())
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

    [Fact]
    public async Task Put_ShouldFail_WhenInvalid()
    {
        // Arrange
        var invalidDistinctionType = "Invalid";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/administration/distinction-types/{invalidDistinctionType}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

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

    [Fact]
    public async Task Put_ShouldFail_WhenAbsent()
    {
        // Arrange
        var absentDistinctionType = "absent-distinction-type";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/administration/distinction-types/{absentDistinctionType}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

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
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenUniqueNameExists()
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

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType, additionalDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/administration/distinction-types/{existingDistinctionType.UniqueName}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

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
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        using (var scope = _factory.CreateMultitenancyScope())
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
    public async Task Put_ShouldFail_WhenUniqueNameNull()
    {
        // Arrange
        var existingDistinctionType = new DistinctionType
        {
            Snowflake = 1,
            UniqueName = $"existing-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/administration/distinction-types/{existingDistinctionType.UniqueName}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putDistinctionType = new
        {
            UniqueName = (string?)null,
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

    [Fact]
    public async Task Put_ShouldFail_WhenUniqueNameEmpty()
    {
        // Arrange
        var existingDistinctionType = new DistinctionType
        {
            Snowflake = 1,
            UniqueName = $"existing-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/administration/distinction-types/{existingDistinctionType.UniqueName}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

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

    [Fact]
    public async Task Put_ShouldFail_WhenUniqueNameTooLong()
    {
        // Arrange
        var existingDistinctionType = new DistinctionType
        {
            Snowflake = 1,
            UniqueName = $"existing-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/administration/distinction-types/{existingDistinctionType.UniqueName}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

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

    [Fact]
    public async Task Put_ShouldFail_WhenUniqueNameInvalid()
    {
        // Arrange
        var existingDistinctionType = new DistinctionType
        {
            Snowflake = 1,
            UniqueName = $"existing-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/administration/distinction-types/{existingDistinctionType.UniqueName}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

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

    [Fact]
    public async Task Put_ShouldFail_WhenDisplayNameNull()
    {
        // Arrange
        var existingDistinctionType = new DistinctionType
        {
            Snowflake = 1,
            UniqueName = $"existing-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/administration/distinction-types/{existingDistinctionType.UniqueName}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putDistinctionType = new
        {
            UniqueName = "put-distinction-type",
            ObjectType = "business-object",
            DisplayName = (string?)null,
        };

        request.Content = JsonContent.Create(putDistinctionType);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenDisplayNameEmpty()
    {
        // Arrange
        var existingDistinctionType = new DistinctionType
        {
            Snowflake = 1,
            UniqueName = $"existing-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/administration/distinction-types/{existingDistinctionType.UniqueName}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

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

    [Fact]
    public async Task Put_ShouldFail_WhenDisplayNameTooLong()
    {
        // Arrange
        var existingDistinctionType = new DistinctionType
        {
            Snowflake = 1,
            UniqueName = $"existing-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/administration/distinction-types/{existingDistinctionType.UniqueName}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

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

    [Fact]
    public async Task Put_ShouldFail_WhenObjectTypeNull()
    {
        // Arrange
        var existingDistinctionType = new DistinctionType
        {
            Snowflake = 1,
            UniqueName = $"existing-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/administration/distinction-types/{existingDistinctionType.UniqueName}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putDistinctionType = new
        {
            UniqueName = "put-distinction-type",
            ObjectType = (string?)null,
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

    [Fact]
    public async Task Put_ShouldFail_WhenObjectTypeEmpty()
    {
        // Arrange
        var existingDistinctionType = new DistinctionType
        {
            Snowflake = 1,
            UniqueName = $"existing-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/administration/distinction-types/{existingDistinctionType.UniqueName}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

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

    [Fact]
    public async Task Put_ShouldFail_WhenObjectTypeInvalid()
    {
        // Arrange
        var existingDistinctionType = new DistinctionType
        {
            Snowflake = 1,
            UniqueName = $"existing-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/administration/distinction-types/{existingDistinctionType.UniqueName}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

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