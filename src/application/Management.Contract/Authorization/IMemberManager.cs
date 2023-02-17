using ChristianSchulz.MultitenancyMonolith.Objects.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authorization;

public interface IMemberManager
{
    ValueTask<Member> GetAsync(long snowflake);

    ValueTask<Member> GetAsync(string member);

    ValueTask<Member?> GetOrDefaultAsync(string member);

    IAsyncEnumerable<Member> GetAsyncEnumerable();

    ValueTask InsertAsync(Member @object);

    ValueTask UpdateAsync(long snowflake, Action<Member> action);

    ValueTask UpdateAsync(string member, Action<Member> action);

    ValueTask DeleteAsync(long snowflake);

    ValueTask DeleteAsync(string member);
}