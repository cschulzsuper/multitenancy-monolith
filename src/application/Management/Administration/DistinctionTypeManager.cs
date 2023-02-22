using ChristianSchulz.MultitenancyMonolith.Application.Authentication;
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

    public async Task<DistinctionType> GetAsync(long snowflake)
    {
        DistinctionTypeValidation.EnsureSnowflake(snowflake);

        var distinctionType = await _repository.GetAsync(snowflake);

        return distinctionType;
    }

    public async Task<DistinctionType> GetAsync(string uniqueName)
    {
        DistinctionTypeValidation.EnsureUniqueName(uniqueName);

        var distinctionType = await _repository.GetAsync(x => x.UniqueName == uniqueName);

        return distinctionType;
    }

    public IAsyncEnumerable<DistinctionType> GetAsyncEnumerable()
        => _repository.GetAsyncEnumerable();

    public async Task InsertAsync(DistinctionType distinctionType)
    {
        DistinctionTypeValidation.EnsureInsertable(distinctionType);

        await _repository.InsertAsync(distinctionType);
    }

    public async Task UpdateAsync(long snowflake, Action<DistinctionType> action)
    {
        IdentityValidation.EnsureSnowflake(snowflake);

        var validatedAction = (DistinctionType distinctionType) =>
        {
            action.Invoke(distinctionType);

            DistinctionTypeValidation.EnsureUpdatable(distinctionType);
        };

        await _repository.UpdateOrThrowAsync(snowflake, validatedAction);
    }

    public async Task UpdateAsync(string uniqueName, Action<DistinctionType> action)
    {
        DistinctionTypeValidation.EnsureUniqueName(uniqueName);

        var validatedAction = (DistinctionType distinctionType) =>
        {
            action.Invoke(distinctionType);

            DistinctionTypeValidation.EnsureUpdatable(distinctionType);
        };

        await _repository.UpdateOrThrowAsync(x => x.UniqueName == uniqueName, validatedAction);
    }

    public async Task DeleteAsync(long snowflake)
    {
        DistinctionTypeValidation.EnsureSnowflake(snowflake);

        await _repository.DeleteOrThrowAsync(snowflake);
    }

    public async Task DeleteAsync(string uniqueName)
    {
        DistinctionTypeValidation.EnsureUniqueName(uniqueName);

        await _repository.DeleteOrThrowAsync(x => x.UniqueName == uniqueName);
    }
}