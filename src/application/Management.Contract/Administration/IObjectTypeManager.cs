using ChristianSchulz.MultitenancyMonolith.Objects.Administration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public interface IObjectTypeManager
{
    Task<bool> ExistsAsync(long snowflake);

    Task<bool> ExistsAsync(string uniqueName);

    Task<ObjectType> GetAsync(long snowflake);

    Task<ObjectType> GetAsync(string uniqueName);

    Task<ObjectType?> GetOrDefaultAsync(long snowflake);

    Task<ObjectType?> GetOrDefaultAsync(string uniqueName);

    IAsyncEnumerable<ObjectType> GetAsyncEnumerable();

    Task InsertAsync(ObjectType objectType);

    Task UpdateAsync(long snowflake, Action<ObjectType> action);

    Task UpdateAsync(string uniqueName, Action<ObjectType> action);

    Task DeleteAsync(long snowflake);

    Task DeleteAsync(string uniqueName);
}