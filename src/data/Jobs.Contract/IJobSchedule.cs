using System;

namespace ChristianSchulz.MultitenancyMonolith.Jobs;

public interface IJobSchedule
{
    public DateTime Next(DateTime @base);
}