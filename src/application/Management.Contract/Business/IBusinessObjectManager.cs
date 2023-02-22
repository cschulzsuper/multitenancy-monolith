using ChristianSchulz.MultitenancyMonolith.Objects.Business;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Business;

public interface IBusinessObjectManager
{
    Task<BusinessObject> GetAsync(long snowflake);

    Task<BusinessObject> GetAsync(string uniqueName);

    IAsyncEnumerable<BusinessObject> GetAsyncEnumerable();

    Task InsertAsync(BusinessObject businessObject);

    Task UpdateAsync(long snowflake, Action<BusinessObject> action);

    Task UpdateAsync(string uniqueName, Action<BusinessObject> action);

    Task DeleteAsync(long snowflake);

    Task DeleteAsync(string uniqueName);
}