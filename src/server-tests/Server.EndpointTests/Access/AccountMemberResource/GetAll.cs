using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using ChristianSchulz.MultitenancyMonolith.Server;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Access.AccountMemberResource;

public sealed class GetAll : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public GetAll(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task GetAll_ShouldSucceed_WhenValid()
    {
        // Arrange
        var existingMember1 = new AccountMember
        {
            UniqueName = $"existing-account-member-1-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        var existingMember2 = new AccountMember
        {
            UniqueName = $"existing-account-member-2-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountMember>>()
                .Insert(existingMember1, existingMember2);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/a1/access/account-members");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonDocument>();
        Assert.NotNull(content);
        Assert.Collection(content.RootElement.EnumerateArray().OrderBy(x => x.GetString("uniqueName")),
            x =>
            {
                Assert.Equal(existingMember1.MailAddress, x.GetString("mailAddress"));
                Assert.Equal(existingMember1.UniqueName, x.GetString("uniqueName"));
            },
            x =>
            {
                Assert.Equal(existingMember2.MailAddress, x.GetString("mailAddress"));
                Assert.Equal(existingMember2.UniqueName, x.GetString("uniqueName"));
            });
    }
}