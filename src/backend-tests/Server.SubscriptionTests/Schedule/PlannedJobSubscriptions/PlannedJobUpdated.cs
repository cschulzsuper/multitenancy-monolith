using ChristianSchulz.MultitenancyMonolith.Backend.Server;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Events;
using ChristianSchulz.MultitenancyMonolith.Jobs;
using ChristianSchulz.MultitenancyMonolith.Objects.Schedule;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Schedule.ConcreteValidators;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Schedule.PlannedJobSubscriptions;

public sealed class PlannedJobUpdated : IClassFixture<WebApplicationFactory<Program>>
{
    private sealed class MockPlannedJobQueue : IPlannedJobQueue
    {
        public PlannedJobRun PlannedJobRun { get; set; }
            = new PlannedJobRun
            {
                UniqueName = "default-job",
                Callback = _ => Task.CompletedTask,
                Timestamp = DateTime.UtcNow.AddMinutes(1)
            };

        public PlannedJobRun Dequeue()
        {
            return PlannedJobRun;
        }

        public void Enqueue(string uniqueName, IPlannedJobSchedule schedule, Func<PlannedJobContext, Task> job)
        {

        }

        public void Requeue(string uniqueName, IPlannedJobSchedule schedule)
        {
            PlannedJobRun = new PlannedJobRun
            {
                UniqueName = uniqueName,
                Callback = _ => Task.CompletedTask,
                Timestamp = DateTime.UtcNow.AddMinutes(1)
            };
        }
    }

    private readonly WebApplicationFactory<Program> _factory;

    public PlannedJobUpdated(
        WebApplicationFactory<Program> factory,
        ITestOutputHelper testOutputHelper)
    {
        _factory = factory.Mock(testOutputHelper)
            .WithWebHostBuilder(app => app
            .ConfigureTestServices(services => services.AddSingleton<IPlannedJobQueue, MockPlannedJobQueue>()));
    }

    [Fact]
    public async Task PlannedJobUpdated_ShouldUpdateQueue()
    {
        // Arrange

        await using var scope = _factory.CreateAsyncMultitenancyScope();

        var existingPlannedJob = new PlannedJob
        {
            UniqueName = "planned-job",
            ExpressionType = ScheduleExpressionTypes.CronExpression,
            Expression = "*/5 * * * *"
        };

        scope.ServiceProvider
            .GetRequiredService<IRepository<PlannedJob>>()
            .Insert(existingPlannedJob);

        // Act
        await scope.ServiceProvider
            .GetRequiredService<IEventSubscriptions>()
            .InvokeAsync("planned-job-updated", scope.ServiceProvider, existingPlannedJob.Snowflake);

        // Assert
        Assert.Equal("planned-job", ((MockPlannedJobQueue)_factory.Services.GetRequiredService<IPlannedJobQueue>()).PlannedJobRun.UniqueName);
    }
}