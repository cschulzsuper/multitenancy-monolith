using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Events;
using ChristianSchulz.MultitenancyMonolith.Objects.Schedule;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Schedule;

internal sealed class PlannedJobManager : IPlannedJobManager
{
    private readonly IRepository<PlannedJob> _repository;
    private readonly IEventStorage _eventStorage;

    public PlannedJobManager(
        IRepository<PlannedJob> repository,
        IEventStorage eventStorage)
    {
        _repository = repository;
        _eventStorage = eventStorage;
    }

    public async Task<PlannedJob> GetAsync(long plannedJob)
    {
        PlannedJobValidation.EnsureJob(plannedJob);

        var @object = await _repository.GetAsync(plannedJob);

        return @object;
    }

    public async Task<PlannedJob> GetAsync(string plannedJob)
    {
        PlannedJobValidation.EnsurePlannedJob(plannedJob);

        var @object = await _repository.GetAsync(x => x.UniqueName == plannedJob);

        return @object;
    }

    public PlannedJob? GetOrDefault(string plannedJob)
    {
        PlannedJobValidation.EnsurePlannedJob(plannedJob);

        var @object = _repository.GetOrDefault(x => x.UniqueName == plannedJob);

        return @object;
    }

    public IAsyncEnumerable<PlannedJob> GetAsyncEnumerable()
        => _repository.GetAsyncEnumerable();

    public void Insert(PlannedJob @object)
    {
        PlannedJobValidation.EnsureInsertable(@object);

        _repository.Insert(@object);

        _eventStorage.Add("planned-job-inserted", @object.Snowflake);
    }

    public async Task InsertAsync(PlannedJob @object)
    {
        PlannedJobValidation.EnsureInsertable(@object);

        await _repository.InsertAsync(@object);

        _eventStorage.Add("planned-job-inserted", @object.Snowflake);
    }

    public async Task UpdateAsync(string plannedJob, Action<PlannedJob> action)
    {
        PlannedJobValidation.EnsurePlannedJob(plannedJob);

        var validatedAction = (PlannedJob @object) =>
        {
            action.Invoke(@object);
            PlannedJobValidation.EnsureUpdatable(@object);
        };

        var snowflake = await _repository.UpdateOrThrowAsync(x => x.UniqueName == plannedJob, validatedAction);

        var unboxedSnowflake = snowflake as long?
            ?? throw new UnreachableException($"Expected snowflake to be of type '{nameof(Int64)}' but found '{snowflake.GetType()}'");

        _eventStorage.Add("planned-job-updated", unboxedSnowflake);
    }
}