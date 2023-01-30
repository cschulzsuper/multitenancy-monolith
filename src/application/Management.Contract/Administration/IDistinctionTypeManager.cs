using ChristianSchulz.MultitenancyMonolith.Objects.Administration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public interface IDistinctionTypeManager
{
    ValueTask<DistinctionType> GetAsync(long snowflake);

    ValueTask<DistinctionType> GetAsync(string uniqueName);

    IAsyncEnumerable<DistinctionType> GetAsyncEnumerable();

    ValueTask InsertAsync(DistinctionType distinctionType);

    ValueTask UpdateAsync(long snowflake, Action<DistinctionType> action);

    ValueTask UpdateAsync(string uniqueName, Action<DistinctionType> action);

    ValueTask DeleteAsync(long snowflake);

    ValueTask DeleteAsync(string uniqueName);
}