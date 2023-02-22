using ChristianSchulz.MultitenancyMonolith.Objects.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authorization;

public interface IMemberManager
{
    Task<Member> GetAsync(long snowflake);

    Task<Member> GetAsync(string member);

    Task<Member?> GetOrDefaultAsync(string member);

    IAsyncEnumerable<Member> GetAsyncEnumerable();

    Task InsertAsync(Member @object);

    Task UpdateAsync(long snowflake, Action<Member> action);

    Task UpdateAsync(string member, Action<Member> action);

    Task DeleteAsync(long snowflake);

    Task DeleteAsync(string member);
}