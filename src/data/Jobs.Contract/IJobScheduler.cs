using System;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Jobs;

public interface IJobScheduler
{
    IJobScheduler WithScheduleResolver(Func<string, IJobSchedule> resolver);

    IJobScheduler Map<THandler>(string uniqueName, Func<THandler, Task> job)
        where THandler : class;
}