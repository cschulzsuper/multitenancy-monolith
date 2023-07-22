using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Jobs;

internal sealed class JobScheduler : IJobScheduler
{
    private readonly IJobQueue _queue;

    private Func<string, IJobSchedule> _scheduleResolver = _ => DefaultSchedule.Instance;

    public JobScheduler(IJobQueue queue)
    {
        _queue = queue;
    }


    public IJobScheduler Map<THandler>(string uniqueName, Func<THandler, Task> job)
        where THandler : class
    {
        var schedule = _scheduleResolver(uniqueName);

        async Task JobAsync(JobContext context)
        {
            var handler = context.Services.GetRequiredService<THandler>();

            await job(handler);
        }

        _queue.Enqueue(uniqueName, schedule, JobAsync);

        return this;
    }

    public IJobScheduler WithScheduleResolver(Func<string, IJobSchedule> resolver)
    {
        _scheduleResolver = resolver;

        return this;
    }

}