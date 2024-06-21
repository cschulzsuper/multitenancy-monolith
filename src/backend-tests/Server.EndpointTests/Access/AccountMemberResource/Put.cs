using ChristianSchulz.MultitenancyMonolith.Backend.Server;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Access.AccountMemberResource;

public sealed class Put 
{
    [Fact]
    public async Task Put_ShouldSucceed_WhenValid()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingAccountMember = new AccountMember
        {
            UniqueName = $"existing-account-member-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountMember>>()
                .Insert(existingAccountMember);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/access/account-members/{existingAccountMember.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putAccountMember = new
        {
            UniqueName = $"put-account-member-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(putAccountMember);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = application.CreateMultitenancyScope())
        {
            var changedMember = scope.ServiceProvider
                .GetRequiredService<IRepository<AccountMember>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingAccountMember.Snowflake &&
                    x.UniqueName == putAccountMember.UniqueName);

            Assert.NotNull(changedMember);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenInvalid()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var invalidAccountMember = "Invalid";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/access/account-members/{invalidAccountMember}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putAccountMember = new
        {
            UniqueName = "put-account-member",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(putAccountMember);

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

        var absentAccountMember = "absent-account-member";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/access/account-members/{absentAccountMember}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putAccountMember = new
        {
            UniqueName = "put-account-member",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(putAccountMember);

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

        var existingAccountMember = new AccountMember
        {
            UniqueName = $"existing-account-member-{Guid.NewGuid()}",
            MailAddress = "default@localhost",
        };

        var additionalMember = new AccountMember
        {
            UniqueName = $"additional-account-member-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountMember>>()
                .Insert(existingAccountMember, additionalMember);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/access/account-members/{existingAccountMember.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putAccountMember = new
        {
            additionalMember.UniqueName,
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(putAccountMember);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        using (var scope = application.CreateMultitenancyScope())
        {
            var unchangedMember = scope.ServiceProvider
                .GetRequiredService<IRepository<AccountMember>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingAccountMember.Snowflake &&
                    x.UniqueName == existingAccountMember.UniqueName);

            Assert.NotNull(unchangedMember);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenUniqueNameNull()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingAccountMember = new AccountMember
        {
            UniqueName = $"existing-account-member-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountMember>>()
                .Insert(existingAccountMember);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/access/account-members/{existingAccountMember.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putAccountMember = new
        {
            UniqueName = (string?)null,
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(putAccountMember);

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

        var existingAccountMember = new AccountMember
        {
            UniqueName = $"existing-account-member-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountMember>>()
                .Insert(existingAccountMember);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/access/account-members/{existingAccountMember.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putAccountMember = new
        {
            UniqueName = string.Empty,
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(putAccountMember);

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

        var existingAccountMember = new AccountMember
        {
            UniqueName = $"existing-account-member-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountMember>>()
                .Insert(existingAccountMember);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/access/account-members/{existingAccountMember.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putAccountMember = new
        {
            UniqueName = new string(Enumerable.Repeat('a', 141).ToArray()),
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(putAccountMember);

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

        var existingAccountMember = new AccountMember
        {
            UniqueName = $"existing-account-member-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountMember>>()
                .Insert(existingAccountMember);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/access/account-members/{existingAccountMember.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putAccountMember = new
        {
            UniqueName = "Invalid",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(putAccountMember);

        var client = application.CreateClient();

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
        using var application = MockWebApplication.Create();

        var existingAccountMember = new AccountMember
        {
            UniqueName = $"existing-account-member-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost"
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountMember>>()
                .Insert(existingAccountMember);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/access/account-members/{existingAccountMember.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putAccountMember = new
        {
            UniqueName = "put-account-member",
            MailAddress = (string?)null
        };

        request.Content = JsonContent.Create(putAccountMember);

        var client = application.CreateClient();

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
        using var application = MockWebApplication.Create();

        var existingAccountMember = new AccountMember
        {
            UniqueName = $"existing-account-member-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost"
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountMember>>()
                .Insert(existingAccountMember);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/access/account-members/{existingAccountMember.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putAccountMember = new
        {
            UniqueName = "put-account-member",
            MailAddress = string.Empty
        };

        request.Content = JsonContent.Create(putAccountMember);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenMailAddressTooLong()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingAccountMember = new AccountMember
        {
            UniqueName = $"existing-account-member-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost"
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountMember>>()
                .Insert(existingAccountMember);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/access/account-members/{existingAccountMember.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putAccountMember = new
        {
            UniqueName = "put-account-member",
            MailAddress = $"{new string(Enumerable.Repeat('a', 64).ToArray())}@{new string(Enumerable.Repeat('a', 190).ToArray())}"
        };

        request.Content = JsonContent.Create(putAccountMember);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenMailAddressLocalPartTooLong()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingAccountMember = new AccountMember
        {
            UniqueName = $"existing-account-member-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost"
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountMember>>()
                .Insert(existingAccountMember);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/access/account-members/{existingAccountMember.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putAccountMember = new
        {
            UniqueName = "put-account-member",
            MailAddress = $"{new string(Enumerable.Repeat('a', 65).ToArray())}@{new string(Enumerable.Repeat('a', 1).ToArray())}"
        };

        request.Content = JsonContent.Create(putAccountMember);

        var client = application.CreateClient();

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
        using var application = MockWebApplication.Create();

        var existingAccountMember = new AccountMember
        {
            UniqueName = $"existing-account-member-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost"
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountMember>>()
                .Insert(existingAccountMember);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/access/account-members/{existingAccountMember.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putAccountMember = new
        {
            UniqueName = "put-account-member",
            MailAddress = "put-foo-bar"
        };

        request.Content = JsonContent.Create(putAccountMember);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}