using System;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Jobs;

public interface IPlannedJobScheduler
{
    IPlannedJobScheduler WithScheduleResolver(Func<string, IPlannedJobSchedule> resolver);

    IPlannedJobScheduler Schedule<THandler>(string uniqueName, Func<THandler, Task> job)
        where THandler : class;

    IPlannedJobScheduler Reschedule(string uniqueName);
}