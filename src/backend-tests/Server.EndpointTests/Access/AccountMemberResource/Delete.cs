using ChristianSchulz.MultitenancyMonolith.Backend.Server;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Access.AccountMemberResource;

public sealed class Delete 
{
    [Fact]
    public async Task Delete_ShouldSucceed_WhenExists()
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

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/a1/access/account-members/{existingAccountMember.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = application.CreateMultitenancyScope())
        {
            var deletedMember = scope.ServiceProvider
                .GetRequiredService<IRepository<AccountMember>>()
                .GetQueryable()
                .SingleOrDefault(x => x.UniqueName == existingAccountMember.UniqueName);

            Assert.Null(deletedMember);
        }
    }

    [Fact]
    public async Task Delete_ShouldFail_WhenAbsent()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var absentAccountMember = "absent-account-member";

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/a1/access/account-members/{absentAccountMember}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Delete_ShouldFail_WhenInvalid()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var invalidAccountMember = "Invalid";

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/a1/access/account-members/{invalidAccountMember}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}