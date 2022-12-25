using ChristianSchulz.MultitenancyMonolith.Aggregates.Administration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public interface IMemberManager
{
    ValueTask<Member> GetAsync(long snowflake);

    ValueTask<Member> GetAsync(string uniqueName);

    IQueryable<Member> GetQueryable();

    ValueTask InsertAsync(Member member);

    ValueTask UpdateAsync(long snowflake, Action<Member> action);

    ValueTask UpdateAsync(string uniqueName, Action<Member> action);

    ValueTask DeleteAsync(long snowflake);

    ValueTask DeleteAsync(string uniqueName);
}