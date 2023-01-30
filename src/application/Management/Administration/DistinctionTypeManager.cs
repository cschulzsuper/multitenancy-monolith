using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Administration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal sealed class DistinctionTypeManager : IDistinctionTypeManager
{
    private readonly IRepository<DistinctionType> _repository;

    public DistinctionTypeManager(IRepository<DistinctionType> repository)
    {
        _repository = repository;
    }

    public async ValueTask<DistinctionType> GetAsync(long snowflake)
    {
        DistinctionTypeValidation.EnsureSnowflake(snowflake);

        var distinctionType = await _repository.GetAsync(snowflake);

        return distinctionType;
    }

    public async ValueTask<DistinctionType> GetAsync(string uniqueName)
    {
        DistinctionTypeValidation.EnsureUniqueName(uniqueName);

        var distinctionType = await _repository.GetAsync(x => x.UniqueName == uniqueName);

        return distinctionType;
    }

    public IAsyncEnumerable<DistinctionType> GetAsyncEnumerable()
        => _repository.GetAsyncEnumerable();

    public async ValueTask InsertAsync(DistinctionType distinctionType)
    {
        DistinctionTypeValidation.EnsureInsertable(distinctionType);

        await _repository.InsertAsync(distinctionType);
    }

    public async ValueTask UpdateAsync(long snowflake, Action<DistinctionType> action)
    {
        var validatedAction = (DistinctionType distinctionType) =>
        {
            action.Invoke(distinctionType);

            DistinctionTypeValidation.EnsureUpdatable(distinctionType);
        };

        await _repository.UpdateOrThrowAsync(snowflake, validatedAction);
    }

    public async ValueTask UpdateAsync(string uniqueName, Action<DistinctionType> action)
    {
        var validatedAction = (DistinctionType distinctionType) =>
        {
            action.Invoke(distinctionType);

            DistinctionTypeValidation.EnsureUpdatable(distinctionType);
        };

        await _repository.UpdateOrThrowAsync(x => x.UniqueName == uniqueName, validatedAction);
    }

    public async ValueTask DeleteAsync(long snowflake)
    {
        DistinctionTypeValidation.EnsureSnowflake(snowflake);

        await _repository.DeleteOrThrowAsync(snowflake);
    }

    public async ValueTask DeleteAsync(string uniqueName)
    {
        DistinctionTypeValidation.EnsureUniqueName(uniqueName);

        await _repository.DeleteOrThrowAsync(x => x.UniqueName == uniqueName);
    }
}