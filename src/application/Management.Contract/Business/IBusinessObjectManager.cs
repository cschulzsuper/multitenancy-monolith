using ChristianSchulz.MultitenancyMonolith.Objects.Business;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Business;

public interface IBusinessObjectManager
{
    ValueTask<BusinessObject> GetAsync(long snowflake);

    ValueTask<BusinessObject> GetAsync(string uniqueName);

    IAsyncEnumerable<BusinessObject> GetAsyncEnumerable();

    ValueTask InsertAsync(BusinessObject businessObject);

    ValueTask UpdateAsync(long snowflake, Action<BusinessObject> action);

    ValueTask UpdateAsync(string uniqueName, Action<BusinessObject> action);

    ValueTask DeleteAsync(long snowflake);

    ValueTask DeleteAsync(string uniqueName);
}