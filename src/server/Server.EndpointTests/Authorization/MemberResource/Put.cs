using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using System.Threading.Tasks;
using System.Net.Http;
using System;
using System.Linq;
using ChristianSchulz.MultitenancyMonolith.Server;
using ChristianSchulz.MultitenancyMonolith.Objects.Authentication;

namespace Authorization.MemberResource;

public sealed class Put : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Put(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Put_ShouldSucceed_WhenValid()
    {
        // Arrange
        var existingMember = new Member
        {
            Snowflake = 1,
            UniqueName = $"existing-member-{Guid.NewGuid()}",
            MailAddress = "default@localhost.local"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<Member>>()
                .Insert(existingMember);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authorization/members/{existingMember.UniqueName}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putMember = new
        {
            UniqueName = $"put-member-{Guid.NewGuid()}",
            MailAddress = "default@localhost.local"
        };

        request.Content = JsonContent.Create(putMember);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var changedMember = scope.ServiceProvider
                .GetRequiredService<IRepository<Member>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingMember.Snowflake &&
                    x.UniqueName == putMember.UniqueName);

            Assert.NotNull(changedMember);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenInvalid()
    {
        // Arrange
        var invalidMember = "Invalid";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authorization/members/{invalidMember}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putMember = new
        {
            UniqueName = "put-member",
            MailAddress = "default@localhost.local"
        };

        request.Content = JsonContent.Create(putMember);

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
        var absentMember = "absent-member";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authorization/members/{absentMember}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putMember = new
        {
            UniqueName = "put-member",
            MailAddress = "default@localhost.local"
        };

        request.Content = JsonContent.Create(putMember);

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
        var existingMember = new Member
        {
            Snowflake = 1,
            UniqueName = $"existing-member-{Guid.NewGuid()}",
            MailAddress = "default@localhost.local",
        };

        var additionalMember = new Member
        {
            Snowflake = 2,
            UniqueName = $"additional-member-{Guid.NewGuid()}",
            MailAddress = "default@localhost.local"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<Member>>()
                .Insert(existingMember, additionalMember);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authorization/members/{existingMember.UniqueName}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putMember = new
        {
            UniqueName = additionalMember.UniqueName,
            MailAddress = "default@localhost.local"
        };

        request.Content = JsonContent.Create(putMember);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var unchangedMember = scope.ServiceProvider
                .GetRequiredService<IRepository<Member>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingMember.Snowflake &&
                    x.UniqueName == existingMember.UniqueName);

            Assert.NotNull(unchangedMember);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenUniqueNameNull()
    {
        // Arrange
        var existingMember = new Member
        {
            Snowflake = 1,
            UniqueName = $"existing-member-{Guid.NewGuid()}",
            MailAddress = "default@localhost.local"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<Member>>()
                .Insert(existingMember);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authorization/members/{existingMember.UniqueName}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putMember = new
        {
            UniqueName = (string?)null,
            MailAddress = "default@localhost.local"
        };

        request.Content = JsonContent.Create(putMember);

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
        var existingMember = new Member
        {
            Snowflake = 1,
            UniqueName = $"existing-member-{Guid.NewGuid()}",
            MailAddress = "default@localhost.local"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<Member>>()
                .Insert(existingMember);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authorization/members/{existingMember.UniqueName}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putMember = new
        {
            UniqueName = string.Empty,
            MailAddress = "default@localhost.local"
        };

        request.Content = JsonContent.Create(putMember);

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
        var existingMember = new Member
        {
            Snowflake = 1,
            UniqueName = $"existing-member-{Guid.NewGuid()}",
            MailAddress = "default@localhost.local"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<Member>>()
                .Insert(existingMember);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authorization/members/{existingMember.UniqueName}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putMember = new
        {
            UniqueName = new string(Enumerable.Repeat('a', 141).ToArray()),
            MailAddress = "default@localhost.local"
        };

        request.Content = JsonContent.Create(putMember);

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
        var existingMember = new Member
        {
            Snowflake = 1,
            UniqueName = $"existing-member-{Guid.NewGuid()}",
            MailAddress = "default@localhost.local"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<Member>>()
                .Insert(existingMember);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authorization/members/{existingMember.UniqueName}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putMember = new
        {
            UniqueName = "Invalid",
            MailAddress = "default@localhost.local"
        };

        request.Content = JsonContent.Create(putMember);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenMailAddressNull()
    {
        // Arrange
        var existingMember = new Member
        {
            Snowflake = 1,
            UniqueName = $"existing-member-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<Member>>()
                .Insert(existingMember);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authorization/members/{existingMember.UniqueName}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putMember = new
        {
            UniqueName = "put-member",
            MailAddress = (string?)null
        };

        request.Content = JsonContent.Create(putMember);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenMailAddressEmpty()
    {
        // Arrange
        var existingMember = new Member
        {
            Snowflake = 1,
            UniqueName = $"existing-member-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<Member>>()
                .Insert(existingMember);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authorization/members/{existingMember.UniqueName}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putMember = new
        {
            UniqueName = "put-member",
            MailAddress = string.Empty
        };

        request.Content = JsonContent.Create(putMember);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenMailAddressTooLongEmpty()
    {
        // Arrange
        var existingMember = new Member
        {
            Snowflake = 1,
            UniqueName = $"existing-member-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<Member>>()
                .Insert(existingMember);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authorization/members/{existingMember.UniqueName}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putMember = new
        {
            UniqueName = "put-member",
            MailAddress = $"{new string(Enumerable.Repeat('a', 64).ToArray())}@{new string(Enumerable.Repeat('a', 190).ToArray())}"
        };

        request.Content = JsonContent.Create(putMember);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenMailAddressLocalPartTooLongEmpty()
    {
        // Arrange
        var existingMember = new Member
        {
            Snowflake = 1,
            UniqueName = $"existing-member-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<Member>>()
                .Insert(existingMember);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authorization/members/{existingMember.UniqueName}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putMember = new
        {
            UniqueName = "put-member",
            MailAddress = $"{new string(Enumerable.Repeat('a', 65).ToArray())}@{new string(Enumerable.Repeat('a', 1).ToArray())}"
        };

        request.Content = JsonContent.Create(putMember);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenMailAddressInvalid()
    {
        // Arrange
        var existingMember = new Member
        {
            Snowflake = 1,
            UniqueName = $"existing-member-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<Member>>()
                .Insert(existingMember);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authorization/members/{existingMember.UniqueName}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putMember = new
        {
            UniqueName = "put-member",
            MailAddress = "put-foo-bar"
        };

        request.Content = JsonContent.Create(putMember);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}