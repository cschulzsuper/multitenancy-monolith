using ChristianSchulz.MultitenancyMonolith.Application.Authentication;
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

        var @object = await _repository.GetAsync(snowflake);

        return @object;
    }

    public async ValueTask<BusinessObject> GetAsync(string businessObject)
    {
        BusinessObjectValidation.EnsureBusinessObject(businessObject);

        var @object = await _repository.GetAsync(x => x.UniqueName == businessObject);

        return @object;
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
        IdentityValidation.EnsureSnowflake(snowflake);

        var validatedAction = (BusinessObject @object) =>
        {
            action.Invoke(@object);

            BusinessObjectValidation.EnsureUpdatable(@object);
        };

        await _repository.UpdateOrThrowAsync(snowflake, validatedAction);
    }

    public async ValueTask UpdateAsync(string businessObject, Action<BusinessObject> action)
    {
        BusinessObjectValidation.EnsureBusinessObject(businessObject);

        var validatedAction = (BusinessObject @object) =>
        {
            action.Invoke(@object);

            BusinessObjectValidation.EnsureUpdatable(@object);
        };

        await _repository.UpdateOrThrowAsync(x => x.UniqueName == businessObject, validatedAction);
    }

    public async ValueTask DeleteAsync(long snowflake)
    {
        BusinessObjectValidation.EnsureSnowflake(snowflake);

        await _repository.DeleteOrThrowAsync(snowflake);
    }

    public async ValueTask DeleteAsync(string businessObject)
    {
        BusinessObjectValidation.EnsureBusinessObject(businessObject);

        await _repository.DeleteOrThrowAsync(x => x.UniqueName == businessObject);
    }
}