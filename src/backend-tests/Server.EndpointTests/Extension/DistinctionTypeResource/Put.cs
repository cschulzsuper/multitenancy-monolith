using ChristianSchulz.MultitenancyMonolith.Backend.Server;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Extension;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Extension.DistinctionTypeResource;

public sealed class Put 
{
    [Fact]
    public async Task Put_ShouldSucceed_WhenObjectTypeBusinessObject()
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

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/distinction-types/{existingDistinctionType.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putDistinctionType = new
        {
            UniqueName = $"put-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Put Distinction Type"
        };

        request.Content = JsonContent.Create(putDistinctionType);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = application.CreateMultitenancyScope())
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
        using var application = MockWebApplication.Create();

        var invalidDistinctionType = "Invalid";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/distinction-types/{invalidDistinctionType}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putDistinctionType = new
        {
            UniqueName = "put-distinction-type",
            ObjectType = "business-object",
            DisplayName = "Put Distinction Type"
        };

        request.Content = JsonContent.Create(putDistinctionType);

        var client = application.CreateClient();

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
        using var application = MockWebApplication.Create();

        var absentDistinctionType = "absent-distinction-type";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/distinction-types/{absentDistinctionType}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putDistinctionType = new
        {
            UniqueName = "put-distinction-type",
            ObjectType = "business-object",
            DisplayName = "Put Distinction Type"
        };

        request.Content = JsonContent.Create(putDistinctionType);

        var client = application.CreateClient();

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
        using var application = MockWebApplication.Create();

        var existingDistinctionType = new DistinctionType
        {
            UniqueName = $"existing-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type"
        };

        var additionalDistinctionType = new DistinctionType
        {
            UniqueName = $"additional-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Additional Distinction Type"
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType, additionalDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/distinction-types/{existingDistinctionType.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putDistinctionType = new
        {
            additionalDistinctionType.UniqueName,
            ObjectType = "business-object",
            DisplayName = "Put Distinction Type"
        };

        request.Content = JsonContent.Create(putDistinctionType);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

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
    public async Task Put_ShouldFail_WhenUniqueNameNull()
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

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/distinction-types/{existingDistinctionType.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putDistinctionType = new
        {
            UniqueName = (string?)null,
            ObjectType = "business-object",
            DisplayName = "Put Distinction Type"
        };

        request.Content = JsonContent.Create(putDistinctionType);

        var client = application.CreateClient();

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

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/distinction-types/{existingDistinctionType.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putDistinctionType = new
        {
            UniqueName = string.Empty,
            ObjectType = "business-object",
            DisplayName = "Put Distinction Type"
        };

        request.Content = JsonContent.Create(putDistinctionType);

        var client = application.CreateClient();

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

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/distinction-types/{existingDistinctionType.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putDistinctionType = new
        {
            UniqueName = new string(Enumerable.Repeat('a', 141).ToArray()),
            ObjectType = "business-object",
            DisplayName = "Put Distinction Type"
        };

        request.Content = JsonContent.Create(putDistinctionType);

        var client = application.CreateClient();

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

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/distinction-types/{existingDistinctionType.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putDistinctionType = new
        {
            UniqueName = "Invalid",
            ObjectType = "business-object",
            DisplayName = "Put Distinction Type"
        };

        request.Content = JsonContent.Create(putDistinctionType);

        var client = application.CreateClient();

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

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/distinction-types/{existingDistinctionType.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putDistinctionType = new
        {
            UniqueName = "put-distinction-type",
            ObjectType = "business-object",
            DisplayName = (string?)null,
        };

        request.Content = JsonContent.Create(putDistinctionType);

        var client = application.CreateClient();

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

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/distinction-types/{existingDistinctionType.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putDistinctionType = new
        {
            UniqueName = "put-distinction-type",
            ObjectType = "business-object",
            DisplayName = string.Empty
        };

        request.Content = JsonContent.Create(putDistinctionType);

        var client = application.CreateClient();

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

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/distinction-types/{existingDistinctionType.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putDistinctionType = new
        {
            UniqueName = "put-distinction-type",
            ObjectType = "put-info@localhost",
            DisplayName = new string(Enumerable.Repeat('a', 141).ToArray()),
        };

        request.Content = JsonContent.Create(putDistinctionType);

        var client = application.CreateClient();

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

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/distinction-types/{existingDistinctionType.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putDistinctionType = new
        {
            UniqueName = "put-distinction-type",
            ObjectType = (string?)null,
            DisplayName = "New Distinction Type"
        };

        request.Content = JsonContent.Create(putDistinctionType);

        var client = application.CreateClient();

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

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/distinction-types/{existingDistinctionType.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putDistinctionType = new
        {
            UniqueName = "put-distinction-type",
            ObjectType = string.Empty,
            DisplayName = "New Distinction Type"
        };

        request.Content = JsonContent.Create(putDistinctionType);

        var client = application.CreateClient();

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

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/distinction-types/{existingDistinctionType.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putDistinctionType = new
        {
            UniqueName = "put-distinction-type",
            ObjectType = "foo-bar",
            DisplayName = "New Distinction Type"
        };

        request.Content = JsonContent.Create(putDistinctionType);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}