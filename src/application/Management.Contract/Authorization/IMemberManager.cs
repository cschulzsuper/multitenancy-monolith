using ChristianSchulz.MultitenancyMonolith.Objects.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authorization;

public interface IMemberManager
{
    ValueTask<Member> GetAsync(long snowflake);

    ValueTask<Member> GetAsync(string uniqueName);

    IAsyncEnumerable<Member> GetAsyncEnumerable();

    ValueTask InsertAsync(Member member);

    ValueTask UpdateAsync(long snowflake, Action<Member> action);

    ValueTask UpdateAsync(string uniqueName, Action<Member> action);

    ValueTask DeleteAsync(long snowflake);

    ValueTask DeleteAsync(string uniqueName);
}