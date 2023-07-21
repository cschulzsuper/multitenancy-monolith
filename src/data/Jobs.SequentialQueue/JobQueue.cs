using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Jobs;

internal sealed class JobQueue : IJobQueue
{
    public readonly Dictionary<string, JobInfo> _jobInfo = new();

    public readonly Dictionary<DateTime, JobInfo> _jobs = new();

    public void Enqueue<THandler>(string uniqueName, IJobSchedule schedule, Func<THandler, Task> job)
        where THandler : class
    {
        _jobInfo.Remove(uniqueName);

        async Task ActionAsync(IServiceProvider services)
        {
            var handler = services.GetRequiredService<THandler>();
            await job(handler);
        }

        var jobInfo = new JobInfo
        {
            UniqueName = uniqueName,
            Job = ActionAsync,
            Schedule = schedule,
        };

        Schedule(jobInfo, DateTime.UtcNow);

        _jobInfo.Add(uniqueName, jobInfo);
    }

    public void Requeue(string uniqueName, IJobSchedule schedule)
    {
        var found = _jobInfo.TryGetValue(uniqueName, out var jobInfo);

        if (!found)
        {
            return;
        }

        var newInfo = jobInfo! with { Schedule = schedule };

        _jobInfo[uniqueName] = newInfo;

        Schedule(jobInfo, DateTime.UtcNow);
    }

    public JobCallback Dequeue()
    {
        var empty = _jobs.Count == 0;
        if (empty)
        {
            var noJobsCallback = new JobCallback
            {
                UniqueName = "no-jobs",
                Job = _ => Task.CompletedTask,
                Timestamp = DateTime.UtcNow.AddMinutes(1)
            };

            return noJobsCallback;
        }

        var job = _jobs.MinBy(x => x.Key);

        _jobs.Remove(job.Key);

        if (!_jobInfo.ContainsValue(job.Value))
        {
            var obsoleteJobCallback = new JobCallback
            {
                UniqueName = "obsolete-job",
                Job = _ => Task.CompletedTask,
                Timestamp = DateTime.UtcNow
            };

            return obsoleteJobCallback;
        }

        Schedule(job.Value, DateTime.UtcNow);

        var jobCallback = new JobCallback
        {
            UniqueName = job.Value.UniqueName,
            Job = job.Value.Job,
            Timestamp = job.Key
        };

        return jobCallback;
    }

    private void Schedule(JobInfo jobInfo, DateTime @base)
    {
        var schedule = jobInfo.Schedule.Next(@base);

        _jobs.Add(schedule, jobInfo);
    }
}