using ChristianSchulz.MultitenancyMonolith.Application.Admission;
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

    public async Task<BusinessObject> GetAsync(long snowflake)
    {
        BusinessObjectValidation.EnsureSnowflake(snowflake);

        var @object = await _repository.GetAsync(snowflake);

        return @object;
    }

    public async Task<BusinessObject> GetAsync(string businessObject)
    {
        BusinessObjectValidation.EnsureBusinessObject(businessObject);

        var @object = await _repository.GetAsync(x => x.UniqueName == businessObject);

        return @object;
    }

    public IAsyncEnumerable<BusinessObject> GetAsyncEnumerable()
        => _repository.GetAsyncEnumerable();

    public async Task InsertAsync(BusinessObject businessObject)
    {
        BusinessObjectValidation.EnsureInsertable(businessObject);

        await _repository.InsertAsync(businessObject);
    }

    public async Task UpdateAsync(long snowflake, Action<BusinessObject> action)
    {
        AuthenticationIdentityValidation.EnsureAuthenticationIdentity(snowflake);

        var validatedAction = (BusinessObject @object) =>
        {
            action.Invoke(@object);

            BusinessObjectValidation.EnsureUpdatable(@object);
        };

        await _repository.UpdateOrThrowAsync(snowflake, validatedAction);
    }

    public async Task UpdateAsync(string businessObject, Action<BusinessObject> action)
    {
        BusinessObjectValidation.EnsureBusinessObject(businessObject);

        var validatedAction = (BusinessObject @object) =>
        {
            action.Invoke(@object);

            BusinessObjectValidation.EnsureUpdatable(@object);
        };

        await _repository.UpdateOrThrowAsync(x => x.UniqueName == businessObject, validatedAction);
    }

    public async Task DeleteAsync(long snowflake)
    {
        BusinessObjectValidation.EnsureSnowflake(snowflake);

        await _repository.DeleteOrThrowAsync(snowflake);
    }

    public async Task DeleteAsync(string businessObject)
    {
        BusinessObjectValidation.EnsureBusinessObject(businessObject);

        await _repository.DeleteOrThrowAsync(x => x.UniqueName == businessObject);
    }
}