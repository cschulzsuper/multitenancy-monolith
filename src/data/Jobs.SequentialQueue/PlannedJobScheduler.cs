using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Jobs;

internal sealed class PlannedJobScheduler : IPlannedJobScheduler
{
    private readonly IPlannedJobQueue _queue;

    private Func<string, IPlannedJobSchedule> _scheduleResolver = _ => DefaultSchedule.Instance;

    public PlannedJobScheduler(IPlannedJobQueue queue)
    {
        _queue = queue;
    }


    public IPlannedJobScheduler Schedule<THandler>(string uniqueName, Func<THandler, Task> job)
        where THandler : class
    {
        var schedule = _scheduleResolver(uniqueName);

        async Task JobAsync(PlannedJobContext context)
        {
            var handler = context.Services.GetRequiredService<THandler>();

            await job(handler);
        }

        _queue.Enqueue(uniqueName, schedule, JobAsync);

        return this;
    }

    public IPlannedJobScheduler Reschedule(string uniqueName)
    {
        var schedule = _scheduleResolver(uniqueName);

        _queue.Requeue(uniqueName, schedule);

        return this;
    }


    public IPlannedJobScheduler WithScheduleResolver(Func<string, IPlannedJobSchedule> resolver)
    {
        _scheduleResolver = resolver;

        return this;
    }

}