using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Business;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Business;

internal sealed class BusinessObjectManager : IBusinessObjectManager
{
    private readonly IRepository<BusinessObject> _repository;

    public BusinessObjectManager(IRepository<BusinessObject> repository)
    {
        _repository = repository;
    }

    public async ValueTask<BusinessObject> GetAsync(long snowflake)
    {
        BusinessObjectValidation.EnsureSnowflake(snowflake);

        var businessObject = await _repository.GetAsync(snowflake);

        return businessObject;
    }

    public async ValueTask<BusinessObject> GetAsync(string uniqueName)
    {
        BusinessObjectValidation.EnsureUniqueName(uniqueName);

        var businessObject = await _repository.GetAsync(x => x.UniqueName == uniqueName);

        return businessObject;
    }

    public IAsyncEnumerable<BusinessObject> GetAsyncEnumerable()
        => _repository.GetAsyncEnumerable();

    public async ValueTask InsertAsync(BusinessObject businessObject)
    {
        BusinessObjectValidation.EnsureInsertable(businessObject);

        await _repository.InsertAsync(businessObject);
    }

    public async ValueTask UpdateAsync(long snowflake, Action<BusinessObject> action)
    {
        var validatedAction = (BusinessObject businessObject) =>
        {
            action.Invoke(businessObject);

            BusinessObjectValidation.EnsureUpdatable(businessObject);
        };

        await _repository.UpdateOrThrowAsync(snowflake, validatedAction);
    }

    public async ValueTask UpdateAsync(string uniqueName, Action<BusinessObject> action)
    {
        var validatedAction = (BusinessObject businessObject) =>
        {
            action.Invoke(businessObject);

            BusinessObjectValidation.EnsureUpdatable(businessObject);
        };

        await _repository.UpdateOrThrowAsync(x => x.UniqueName == uniqueName, validatedAction);
    }

    public async ValueTask DeleteAsync(long snowflake)
    {
        BusinessObjectValidation.EnsureSnowflake(snowflake);

        await _repository.DeleteOrThrowAsync(snowflake);
    }

    public async ValueTask DeleteAsync(string uniqueName)
    {
        BusinessObjectValidation.EnsureUniqueName(uniqueName);

        await _repository.DeleteOrThrowAsync(x => x.UniqueName == uniqueName);
    }
}