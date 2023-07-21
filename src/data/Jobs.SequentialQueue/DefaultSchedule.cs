using Cronos;
using System;
using System.Diagnostics;

namespace ChristianSchulz.MultitenancyMonolith.Jobs;

internal sealed class DefaultSchedule : IJobSchedule
{

    public static readonly IJobSchedule Instance = new DefaultSchedule();

    private const string _fiveMinuteInterval = "*/1 * * * *";

    private static readonly CronExpression _schedule = CronExpression.Parse(_fiveMinuteInterval);

    public DateTime Next(DateTime @base)
    {
        var nextOccurrence = _schedule.GetNextOccurrence(@base);

        if (nextOccurrence != null)
        {
            return nextOccurrence.Value;
        }

        throw new UnreachableException($"Unable to calculate next occurrence of 5 minute interval '{_fiveMinuteInterval}' after '{@base:O}'.");
    }
}
