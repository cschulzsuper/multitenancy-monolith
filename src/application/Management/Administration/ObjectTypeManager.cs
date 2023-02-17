using ChristianSchulz.MultitenancyMonolith.Application.Authentication;
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

    public async ValueTask<bool> ExistsAsync(long snowflake)
    {
        var objectType = await _repository.ExistsAsync(snowflake);

        return objectType;
    }

    public async ValueTask<bool> ExistsAsync(string uniqueName)
    {
        ObjectTypeValidation.EnsureUniqueName(uniqueName);

        var objectType = await _repository.ExistsAsync(x => x.UniqueName == uniqueName);

        return objectType;
    }

    public async ValueTask<ObjectType> GetAsync(long snowflake)
    {
        ObjectTypeValidation.EnsureSnowflake(snowflake);

        var objectType = await _repository.GetAsync(snowflake);

        return objectType;
    }

    public async ValueTask<ObjectType> GetAsync(string uniqueName)
    {
        ObjectTypeValidation.EnsureUniqueName(uniqueName);

        var objectType = await _repository.GetAsync(x => x.UniqueName == uniqueName);

        return objectType;
    }

    public async ValueTask<ObjectType?> GetOrDefaultAsync(long snowflake)
    {
        ObjectTypeValidation.EnsureSnowflake(snowflake);

        var objectType = await _repository.GetOrDefaultAsync(snowflake);

        return objectType;
    }

    public async ValueTask<ObjectType?> GetOrDefaultAsync(string uniqueName)
    {
        ObjectTypeValidation.EnsureUniqueName(uniqueName);

        var objectType = await _repository.GetOrDefaultAsync(x => x.UniqueName == uniqueName);

        return objectType;
    }

    public IAsyncEnumerable<ObjectType> GetAsyncEnumerable()
        => _repository.GetAsyncEnumerable();

    public async ValueTask InsertAsync(ObjectType objectType)
    {
        ObjectTypeValidation.EnsureInsertable(objectType);

        await _repository.InsertAsync(objectType);
    }

    public async ValueTask UpdateAsync(long snowflake, Action<ObjectType> action)
    {
        IdentityValidation.EnsureSnowflake(snowflake);

        var validatedAction = (ObjectType objectType) =>
        {
            action.Invoke(objectType);

            ObjectTypeValidation.EnsureUpdatable(objectType);
        };

        await _repository.UpdateOrThrowAsync(snowflake, validatedAction);
    }

    public async ValueTask UpdateAsync(string uniqueName, Action<ObjectType> action)
    {
        ObjectTypeValidation.EnsureUniqueName(uniqueName);

        var validatedAction = (ObjectType objectType) =>
        {
            action.Invoke(objectType);

            ObjectTypeValidation.EnsureUpdatable(objectType);
        };

        await _repository.UpdateOrThrowAsync(x => x.UniqueName == uniqueName, validatedAction);
    }

    public async ValueTask DeleteAsync(long snowflake)
    {
        ObjectTypeValidation.EnsureSnowflake(snowflake);

        await _repository.DeleteOrThrowAsync(snowflake);
    }

    public async ValueTask DeleteAsync(string uniqueName)
    {
        ObjectTypeValidation.EnsureUniqueName(uniqueName);

        await _repository.DeleteOrThrowAsync(x => x.UniqueName == uniqueName);
    }
}