using ChristianSchulz.MultitenancyMonolith.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using ChristianSchulz.MultitenancyMonolith.Objects.Schedule;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authentication;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Schedule.ConcreteValidators;

namespace Schedule.PlannedJobResource
{
    public sealed class GetAll : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public GetAll(WebApplicationFactory<Program> factory)
        {
            _factory = factory.Mock();
        }

        [Fact]
        public async Task GetAll_ShouldSucceed()
        {
            // Arrange
            var existingPlannedJob1 = new PlannedJob
            {
                UniqueName = "mock-job-1",
                Expression = "*/1 * * * *",
                ExpressionType = ScheduleExpressionTypes.CronExpression
            };

            var existingPlannedJob2 = new PlannedJob
            {
                UniqueName = "mock-job-2",
                Expression = "*/1 * * * *",
                ExpressionType = ScheduleExpressionTypes.CronExpression
            };

            using (var scope = _factory.CreateMultitenancyScope())
            {
                scope.ServiceProvider
                    .GetRequiredService<IRepository<PlannedJob>>()
                    .Delete(x => true);

                scope.ServiceProvider
                    .GetRequiredService<IRepository<PlannedJob>>()
                    .Insert(existingPlannedJob1, existingPlannedJob2);
            }

            var request = new HttpRequestMessage(HttpMethod.Get, "/api/schedule/planned-jobs");
            request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

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
                    Assert.Equal(existingPlannedJob1.UniqueName, x.GetString("uniqueName"));
                    Assert.Equal(existingPlannedJob1.Expression, x.GetString("expression"));
                },
                x =>
                {
                    Assert.Equal(existingPlannedJob2.UniqueName, x.GetString("uniqueName"));
                    Assert.Equal(existingPlannedJob2.Expression, x.GetString("expression"));
                });
        }
    }
}
