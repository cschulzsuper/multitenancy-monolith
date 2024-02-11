using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Jobs;

internal sealed class PlannedJobQueue : IPlannedJobQueue
{
    public readonly Dictionary<string, PlannedJobInfo> _jobInfo = new();

    public readonly Dictionary<DateTime, PlannedJobInfo> _jobs = new();

    public void Enqueue(string uniqueName, IPlannedJobSchedule schedule, Func<PlannedJobContext, Task> job)
    {
        _jobInfo.Remove(uniqueName);

        async Task JobAsync(IServiceProvider services)
        {
            var context = new PlannedJobContext 
            { 
                Services = services
            };

            await job(context);
        }

        var jobInfo = new PlannedJobInfo
        {
            UniqueName = uniqueName,
            Job = JobAsync,
            Schedule = schedule,
        };

        Schedule(jobInfo, DateTime.UtcNow);

        _jobInfo.Add(uniqueName, jobInfo);
    }

    public void Requeue(string uniqueName, IPlannedJobSchedule schedule)
    {
        var found = _jobInfo.TryGetValue(uniqueName, out var jobInfo);

        if (!found)
        {
            return;
        }

        var newInfo = jobInfo! with { Schedule = schedule };

        _jobInfo[uniqueName] = newInfo;

        Schedule(newInfo, DateTime.UtcNow);
    }

    public PlannedJobRun Dequeue()
    {
        var empty = _jobs.Count == 0;
        if (empty)
        {
            var delayJobRun = new PlannedJobRun
            {
                UniqueName = "no-jobs",
                Callback = _ => Task.CompletedTask,
                Timestamp = DateTime.UtcNow.AddMinutes(1)
            };

            return delayJobRun;
        }

        var job = _jobs.MinBy(x => x.Key);

        _jobs.Remove(job.Key);

        if (!_jobInfo.ContainsValue(job.Value))
        {
            var obsoleteJobRun = new PlannedJobRun
            {
                UniqueName = "obsolete-job",
                Callback = _ => Task.CompletedTask,
                Timestamp = DateTime.UtcNow
            };

            return obsoleteJobRun;
        }

        Schedule(job.Value, job.Key);

        var plannedJobRun = new PlannedJobRun
        {
            UniqueName = job.Value.UniqueName,
            Callback = job.Value.Job,
            Timestamp = job.Key
        };

        return plannedJobRun;
    }

    private void Schedule(PlannedJobInfo jobInfo, DateTime last)
    {
        var now = DateTime.UtcNow;
        var @base = last > now ? last : now;

        var schedule = jobInfo.Schedule.Next(@base);

        while(_jobs.ContainsKey(schedule))
        {
            schedule = schedule.AddTicks(1);
        }

        _jobs.Add(schedule, jobInfo);
    }
}