using System;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Jobs;

public interface IPlannedJobQueue
{
    void Enqueue(string uniqueName, IPlannedJobSchedule schedule, Func<PlannedJobContext, Task> job);

    void Requeue(string uniqueName, IPlannedJobSchedule schedule);

    PlannedJobRun Dequeue();
}