using Cronos;
using System;
using System.Diagnostics;

namespace ChristianSchulz.MultitenancyMonolith.Jobs;

internal sealed class DefaultSchedule : IPlannedJobSchedule
{

    public static readonly IPlannedJobSchedule Instance = new DefaultSchedule();

    private const string _fifeMinuteInterval = "*/5 * * * *";

    private static readonly CronExpression _schedule = CronExpression.Parse(_fifeMinuteInterval);

    public DateTime Next(DateTime @base)
    {
        var adjustedBase = @base;

        var nextOccurrence = _schedule.GetNextOccurrence(adjustedBase);

        if (nextOccurrence != null)
        {
            return nextOccurrence.Value;
        }

        throw new UnreachableException($"Unable to calculate next occurrence of 5 minute interval '{_fifeMinuteInterval}' after '{@base:O}'.");
    }
}