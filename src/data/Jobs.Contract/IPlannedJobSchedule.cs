using System;

namespace ChristianSchulz.MultitenancyMonolith.Jobs;

public interface IPlannedJobSchedule
{
    public DateTime Next(DateTime @base);
}