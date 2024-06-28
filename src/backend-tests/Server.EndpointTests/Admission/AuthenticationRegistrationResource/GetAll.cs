using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Admission.ConcreteValidators;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Admission.AuthenticationRegistrationResource;

public sealed class GetAll 
{
    [Fact]
    public async Task GetAll_ShouldSucceed_WhenValid()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingRegistration1 = new AuthenticationRegistration
        {
            AuthenticationIdentity = $"existing-authentication-registration-authentication-identity-{Guid.NewGuid()}",
            ProcessState = AuthenticationRegistrationProcessStates.New,
            ProcessToken = Guid.NewGuid(),
            Secret = $"{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        var existingRegistration2 = new AuthenticationRegistration
        {
            AuthenticationIdentity = $"existing-authentication-registration-authentication-identity-{Guid.NewGuid()}",
            ProcessState = AuthenticationRegistrationProcessStates.New,
            ProcessToken = Guid.NewGuid(),
            Secret = $"{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationRegistration>>()
                .Insert(existingRegistration1, existingRegistration2);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/a1/admission/authentication-registrations");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonDocument>();
        Assert.NotNull(content);
        Assert.Collection(content.RootElement.EnumerateArray().OrderBy(x => x.GetString("snowflake")),
            x =>
            {
                Assert.Equal(existingRegistration1.MailAddress, x.GetString("mailAddress"));
                Assert.Equal(existingRegistration1.Snowflake, x.GetProperty("snowflake").GetInt64());
                Assert.Equal(existingRegistration1.AuthenticationIdentity, x.GetString("authenticationIdentity"));
                Assert.Equal(AuthenticationRegistrationProcessStates.New, x.GetString("processState"));
            },
            x =>
            {
                Assert.Equal(existingRegistration2.MailAddress, x.GetString("mailAddress"));
                Assert.Equal(existingRegistration2.Snowflake, x.GetProperty("snowflake").GetInt64());
                Assert.Equal(existingRegistration2.AuthenticationIdentity, x.GetString("authenticationIdentity"));
                Assert.Equal(AuthenticationRegistrationProcessStates.New, x.GetString("processState"));
            });
    }
}