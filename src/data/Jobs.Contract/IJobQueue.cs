using System;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Jobs;

public interface IJobQueue
{
    void Enqueue<THandler>(string uniqueName, IJobSchedule schedule, Func<THandler, Task> job)
        where THandler : class;

    void Requeue(string uniqueName, IJobSchedule schedule);

    JobCallback Dequeue();
}
