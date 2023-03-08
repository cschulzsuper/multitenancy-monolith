using ChristianSchulz.MultitenancyMonolith.Application.Admission;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Administration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal sealed class ObjectTypeManager : IObjectTypeManager
{
    private readonly IRepository<ObjectType> _repository;

    public ObjectTypeManager(IRepository<ObjectType> repository)
    {
        _repository = repository;
    }

    public async Task<bool> ExistsAsync(long snowflake)
    {
        var objectType = await _repository.ExistsAsync(snowflake);

        return objectType;
    }

    public async Task<bool> ExistsAsync(string uniqueName)
    {
        ObjectTypeValidation.EnsureUniqueName(uniqueName);

        var objectType = await _repository.ExistsAsync(x => x.UniqueName == uniqueName);

        return objectType;
    }

    public async Task<ObjectType> GetAsync(long snowflake)
    {
        ObjectTypeValidation.EnsureSnowflake(snowflake);

        var objectType = await _repository.GetAsync(snowflake);

        return objectType;
    }

    public async Task<ObjectType> GetAsync(string uniqueName)
    {
        ObjectTypeValidation.EnsureUniqueName(uniqueName);

        var objectType = await _repository.GetAsync(x => x.UniqueName == uniqueName);

        return objectType;
    }

    public async Task<ObjectType?> GetOrDefaultAsync(long snowflake)
    {
        ObjectTypeValidation.EnsureSnowflake(snowflake);

        var objectType = await _repository.GetOrDefaultAsync(snowflake);

        return objectType;
    }

    public async Task<ObjectType?> GetOrDefaultAsync(string uniqueName)
    {
        ObjectTypeValidation.EnsureUniqueName(uniqueName);

        var objectType = await _repository.GetOrDefaultAsync(x => x.UniqueName == uniqueName);

        return objectType;
    }

    public IAsyncEnumerable<ObjectType> GetAsyncEnumerable()
        => _repository.GetAsyncEnumerable();

    public async Task InsertAsync(ObjectType objectType)
    {
        ObjectTypeValidation.EnsureInsertable(objectType);

        await _repository.InsertAsync(objectType);
    }

    public async Task UpdateAsync(long snowflake, Action<ObjectType> action)
    {
        AuthenticationIdentityValidation.EnsureAuthenticationIdentity(snowflake);

        var validatedAction = (ObjectType objectType) =>
        {
            action.Invoke(objectType);

            ObjectTypeValidation.EnsureUpdatable(objectType);
        };

        await _repository.UpdateOrThrowAsync(snowflake, validatedAction);
    }

    public async Task UpdateAsync(string uniqueName, Action<ObjectType> action)
    {
        ObjectTypeValidation.EnsureUniqueName(uniqueName);

        var validatedAction = (ObjectType objectType) =>
        {
            action.Invoke(objectType);

            ObjectTypeValidation.EnsureUpdatable(objectType);
        };

        await _repository.UpdateOrThrowAsync(x => x.UniqueName == uniqueName, validatedAction);
    }

    public async Task DeleteAsync(long snowflake)
    {
        ObjectTypeValidation.EnsureSnowflake(snowflake);

        await _repository.DeleteOrThrowAsync(snowflake);
    }

    public async Task DeleteAsync(string uniqueName)
    {
        ObjectTypeValidation.EnsureUniqueName(uniqueName);

        await _repository.DeleteOrThrowAsync(x => x.UniqueName == uniqueName);
    }
}