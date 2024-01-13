using ChristianSchulz.MultitenancyMonolith.Events;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Schedule;

[SuppressMessage("Style", "IDE1006:NamingRuleViolation")]
public static class _Subscriptions
{
    public static IEventSubscriptions MapScheduleSubscriptions(this IEventSubscriptions subscriptions)
    {
        subscriptions.MapPlannedJobSubscriptions();

        return subscriptions;
    }
}