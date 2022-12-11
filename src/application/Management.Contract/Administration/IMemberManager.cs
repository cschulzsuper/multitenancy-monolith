using ChristianSchulz.MultitenancyMonolith.Aggregates.Administration;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public interface IMemberManager
{
    Member Get(long snowflake);

    Member Get(string uniqueName);

    IQueryable<Member> GetAll();

    void Insert(Member member);
}