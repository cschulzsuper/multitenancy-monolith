using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Events;
using ChristianSchulz.MultitenancyMonolith.Objects.Schedule;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Schedule;

internal sealed class JobManager : IJobManager
{
    private readonly IRepository<Job> _repository;
    private readonly IEventStorage _eventStorage;

    public JobManager(
        IRepository<Job> repository,
        IEventStorage eventStorage)
    {
        _repository = repository;
        _eventStorage = eventStorage;
    }

    public async Task<Job> GetAsync(string job)
    {
        JobValidation.EnsureJob(job);

        var @object = await _repository.GetAsync(job);

        return @object;
    }

    public IAsyncEnumerable<Job> GetAsyncEnumerable(Func<IQueryable<Job>, IQueryable<Job>> query)
        => _repository.GetAsyncEnumerable(query);

    public async Task InsertAsync(Job @object)
    {
        JobValidation.EnsureInsertable(@object);

        await _repository.InsertAsync(@object);

        _eventStorage.Add("job-inserted", @object.Snowflake);
    }

    public async Task UpdateAsync(string job, Action<Job> action)
    {
        JobValidation.EnsureJob(job);

        var validatedAction = (Job @object) =>
        {
            action.Invoke(@object);
            JobValidation.EnsureUpdatable(@object);
        };

        var snowflake = await _repository.UpdateOrThrowAsync(x => x.UniqueName == job, validatedAction);

        var unboxedSnowflake = snowflake as long?
            ?? throw new UnreachableException($"Expected snowflake to be of type '{nameof(Int64)}' but found '{snowflake.GetType()}'");

        _eventStorage.Add("job-updated", unboxedSnowflake);
    }
}