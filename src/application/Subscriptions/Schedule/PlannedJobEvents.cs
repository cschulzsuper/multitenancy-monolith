using ChristianSchulz.MultitenancyMonolith.Events;
using System;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Schedule;

public static class PlannedJobEvents
{
    public static IEventSubscriptions MapPlannedJobSubscriptions(this IEventSubscriptions subscriptions)
    {
        subscriptions.Map("planned-job-updated", Updated);

        return subscriptions;
    }

    private static Func<IPlannedJobRescheduler, long, Task> Updated =>
        (rescheduler, plannedJob)
            => rescheduler.RescheduleAsync(plannedJob);
}