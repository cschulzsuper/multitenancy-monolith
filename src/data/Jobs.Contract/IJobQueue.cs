using System;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Jobs;

public interface IJobQueue
{
    void Enqueue(string uniqueName, IJobSchedule schedule, Func<JobContext, Task> job);

    void Requeue(string uniqueName);

    void Requeue(string uniqueName, IJobSchedule schedule);

    JobInvocation Dequeue();
}