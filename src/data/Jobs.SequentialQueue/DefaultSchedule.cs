using Cronos;
using System;
using System.Diagnostics;

namespace ChristianSchulz.MultitenancyMonolith.Jobs;

internal sealed class DefaultSchedule : IJobSchedule
{

    public static readonly IJobSchedule Instance = new DefaultSchedule();

    private const string _twoMinuteInterval = "*/2 * * * *";

    private static readonly CronExpression _schedule = CronExpression.Parse(_twoMinuteInterval);

    public DateTime Next(DateTime @base)
    {
        var adjustedBase = @base;

        var nextOccurrence = _schedule.GetNextOccurrence(adjustedBase);

        if (nextOccurrence != null)
        {
            return nextOccurrence.Value;
        }

        throw new UnreachableException($"Unable to calculate next occurrence of 2 minute interval '{_twoMinuteInterval}' after '{@base:O}'.");
    }
}