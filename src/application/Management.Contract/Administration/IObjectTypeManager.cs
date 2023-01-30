using ChristianSchulz.MultitenancyMonolith.Objects.Administration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public interface IObjectTypeManager
{
    ValueTask<bool> ExistsAsync(long snowflake);

    ValueTask<bool> ExistsAsync(string uniqueName);

    ValueTask<ObjectType> GetAsync(long snowflake);

    ValueTask<ObjectType> GetAsync(string uniqueName);

    ValueTask<ObjectType?> GetOrDefaultAsync(long snowflake);

    ValueTask<ObjectType?> GetOrDefaultAsync(string uniqueName);

    IAsyncEnumerable<ObjectType> GetAsyncEnumerable();

    ValueTask InsertAsync(ObjectType objectType);

    ValueTask UpdateAsync(long snowflake, Action<ObjectType> action);

    ValueTask UpdateAsync(string uniqueName, Action<ObjectType> action);

    ValueTask DeleteAsync(long snowflake);

    ValueTask DeleteAsync(string uniqueName);
}