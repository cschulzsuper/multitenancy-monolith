using ChristianSchulz.MultitenancyMonolith.Objects.Extension;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Extension;

public interface IDistinctionTypeManager
{
    Task<DistinctionType> GetAsync(long snowflake);

    Task<DistinctionType> GetAsync(string uniqueName);

    IAsyncEnumerable<DistinctionType> GetAsyncEnumerable();

    Task InsertAsync(DistinctionType distinctionType);

    Task UpdateAsync(long snowflake, Action<DistinctionType> action);

    Task UpdateAsync(string uniqueName, Action<DistinctionType> action);

    Task DeleteAsync(long snowflake);

    Task DeleteAsync(string uniqueName);
}