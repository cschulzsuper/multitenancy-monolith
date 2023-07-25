using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ChristianSchulz.MultitenancyMonolith.Objects.Schedule;

namespace ChristianSchulz.MultitenancyMonolith.Application.Schedule;

public interface IPlannedJobManager
{
    Task<PlannedJob> GetAsync(long job);

    Task<PlannedJob> GetAsync(string job);

    PlannedJob? GetOrDefault(string job);

    IAsyncEnumerable<PlannedJob> GetAsyncEnumerable();

    void Insert(PlannedJob @object);

    Task InsertAsync(PlannedJob @object);

    Task UpdateAsync(string job, Action<PlannedJob> action);
}