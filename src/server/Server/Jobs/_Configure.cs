using ChristianSchulz.MultitenancyMonolith.Application.Schedule;
using ChristianSchulz.MultitenancyMonolith.Events;
using ChristianSchulz.MultitenancyMonolith.Jobs;
using ChristianSchulz.MultitenancyMonolith.Objects.Schedule;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Schedule.ConcreteValidators;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Server.Jobs;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
internal static class _Configure
{
    public static IPlannedJobScheduler ConfigureJobScheduler(this IApplicationBuilder app)
    {
        var scheduler = app.ApplicationServices
            .GetRequiredService<IPlannedJobScheduler>()
            .WithScheduleResolver(job =>
            {
                using var scope = app.ApplicationServices.CreateScope();

                var jobManager = scope.ServiceProvider.GetRequiredService<IPlannedJobManager>();

                var @object = jobManager.GetOrDefault(job);
                if (@object == null)
                {
                    @object = new PlannedJob
                    {
                        UniqueName = job,
                        ExpressionType = ScheduleExpressionTypes.CronExpression,
                        Expression = "*/5 * * * *"
                    };

                    jobManager.Insert(@object);
                }

                if(@object.ExpressionType != ScheduleExpressionTypes.CronExpression)
                {
                    throw new UnreachableException("Only cron expressions are supported.");
                }

                return new CronExpressionSchedule(@object.Expression);
            });

        return scheduler;
    }

    public static PlannedJobsOptions Configure(this PlannedJobsOptions options)
    {
        options.AfterJobInvocation = AfterJobInvocation;

        return options;
    }

    private static async Task AfterJobInvocation(IServiceProvider services, string _)
    {
        await services
            .GetRequiredService<IEventStorage>()
            .FlushAsync();
    }
}